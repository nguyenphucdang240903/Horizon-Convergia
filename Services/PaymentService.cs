using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using DataAccessObjects.Setting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PayOS _payos;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IOptions<PayOSSettings> options, IUnitOfWork unitOfWork)
        {
            var settings = options.Value;
            _payos = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateMultiOrderPayOSUrlAsync(CreatePaymentRequestDTO dto, string userId)
        {
            var orders = await _unitOfWork.Repository<Order>()
                .Query()
                .Where(o => dto.OrderIds.Contains(o.Id) && !o.IsDeleted)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            if (orders == null || !orders.Any())
                throw new Exception("No orders found");

            if (orders.Any(o => o.BuyerId != userId))
                throw new Exception("You are not authorized to pay for one or more of these orders");

            if (_unitOfWork.Repository<Payment>()
                .Query()
                .Any(p => dto.OrderIds.Contains(p.OrderId) && p.PaymentStatus == PaymentStatus.Pending))
                throw new Exception("One or more orders already have a pending payment");

            var totalAmount = orders.Sum(o => o.TotalPrice);
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var returnUrl = "https://localhost:7076/api/Payments/payos-callback";
            var description = $"Thanh toán đơn hàng {orders.Count} đơn";

            var items = orders.Select(order => new ItemData(
                name: $"Đơn hàng {order.OrderNo}",
                quantity: 1,
                price: (int)order.TotalPrice
            )).ToList();

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)totalAmount,
                description: description,
                items: items,
                returnUrl: returnUrl,
                cancelUrl: returnUrl
            );

            var paymentResult = await _payos.createPaymentLink(paymentData);

            // Ghi nhận payment cho tất cả order
            foreach (var order in orders)
            {
                var payment = new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = order.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderId = order.Id,
                    UserId = userId,
                    PaymentMethod = "PayOS",
                    Reference = orderCode.ToString(),
                    Description = description,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentType = PaymentType.BuyerPayment
                };

                await _unitOfWork.Repository<Payment>().AddAsync(payment);
            }

            await _unitOfWork.SaveAsync();

            return paymentResult.checkoutUrl;
        }

        public async Task<bool> HandlePayOSCallbackAsync(PayOSReturnDTO dto)
        {
            var payments = _unitOfWork.Repository<Payment>()
                .Query()
                .Where(p => p.Reference == dto.OrderCode)
                .ToList();

            if (payments == null || !payments.Any()) return false;

            var success = dto.Status == "PAID";
            var transactionStatus = success ? TransactionStatus.Success : TransactionStatus.Failed;

            foreach (var payment in payments)
            {
                payment.PaymentStatus = success ? PaymentStatus.Completed : PaymentStatus.Failed;
                payment.UpdatedAt = DateTime.UtcNow;
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(payment.UserId);

                var transaction = new PaymentTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    PaymentId = payment.Id,
                    Amount = payment.Amount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Reference = dto.id,
                    TransactionType = "PayOS",
                    TransactionStatus = transactionStatus,
                    UserId = payment.UserId,

                };

                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);

                if (payment.PaymentType == PaymentType.BuyerPayment)
                {
                    var order = await _unitOfWork.Repository<Order>().GetByIdAsync(payment.OrderId);
                    if (order != null && success)
                    {
                        order.Status = OrderStatus.Confirmed;
                        order.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<Order>().Update(order);
                    }
                }
                else if (payment.PaymentType == PaymentType.SellerPayment)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(payment.ProductId);
                    if (product != null && success)
                    {
                        product.Status = ProductStatus.Active;
                        product.IsVerified = true;
                        product.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<Product>().Update(product);
                    }
                }

                _unitOfWork.Repository<Payment>().Update(payment);
            }

            await _unitOfWork.SaveAsync();
            return success;
        }
    }
}
