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

        public async Task<string> CreatePayOSUrlAsync(CreatePaymentRequestDTO dto, string userId)
        {
            var order = await _unitOfWork.Repository<Order>()
                .Query()
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId && !o.IsDeleted);

            if (order == null)
                throw new Exception("Order not found");

            if (order.BuyerId != userId)
                throw new Exception("You are not authorized to pay for this order");

            var existingPayment = _unitOfWork.Repository<Payment>()
                .Query()
                .FirstOrDefault(p => p.OrderId == dto.OrderId && p.PaymentStatus == PaymentStatus.Pending);

            if (existingPayment != null)
                throw new Exception("This order already has a pending payment");

            var returnUrl = "https://localhost:7076/api/Payments/payos-callback";
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var description = $"Thanh toán đơn hàng";
            var items = new List<ItemData>
    {
        new ItemData(
            name: $"Đơn hàng {order.OrderNo}",
            quantity: 1,
            price: (int)order.TotalPrice
        )
    };

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)order.TotalPrice,
                description: description,
                items: items,
                returnUrl: returnUrl,
                cancelUrl: returnUrl
            );

            var paymentResult = await _payos.createPaymentLink(paymentData);

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
            await _unitOfWork.SaveAsync();

            return paymentResult.checkoutUrl;
        }




        public async Task<bool> HandlePayOSCallbackAsync(PayOSReturnDTO dto)
        {
            var payment = _unitOfWork.Repository<Payment>()
                .Query()
                .FirstOrDefault(p => p.Reference == dto.OrderCode);

            if (payment == null) return false;

            var transaction = new PaymentTransaction
            {
                Id = Guid.NewGuid().ToString(),
                PaymentId = payment.Id,
                Amount = payment.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Reference = dto.id,
                TransactionType = "PayOS",
                TransactionStatus = dto.Status == "PAID" ? TransactionStatus.Success : TransactionStatus.Failed,
                UserId = payment.UserId
            };

            payment.PaymentStatus = transaction.TransactionStatus == TransactionStatus.Success
                ? PaymentStatus.Completed
                : PaymentStatus.Failed;
            payment.UpdatedAt = DateTime.UtcNow;

            if (payment.PaymentType == PaymentType.BuyerPayment)
            {
                // Buyer thanh toán đơn hàng
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(payment.OrderId);
                if (order != null && transaction.TransactionStatus == TransactionStatus.Success)
                {
                    order.Status = OrderStatus.Confirmed; // Đơn hàng chờ seller xác nhận
                    order.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<Order>().Update(order);
                }
            }
            else if (payment.PaymentType == PaymentType.SellerPayment)
            {
                // Seller thanh toán để kích hoạt sản phẩm
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(payment.ProductId);
                if (product != null && transaction.TransactionStatus == TransactionStatus.Success)
                {
                    product.Status = ProductStatus.Active;
                    product.IsVerified = true;
                    product.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<Product>().Update(product);
                }
            }

            await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
            _unitOfWork.Repository<Payment>().Update(payment);
            await _unitOfWork.SaveAsync();

            return transaction.TransactionStatus == TransactionStatus.Success;
        }
    }
}
