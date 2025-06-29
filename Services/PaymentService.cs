using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using DataAccessObjects.Setting;
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
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var description = dto.Description ?? "Thanh toán đơn hàng HorizonConvergia";

            // Tạo item cho hóa đơn
            var items = new List<ItemData>
            {
                new ItemData(
                    name: "Thanh toán HorizonConvergia",
                    quantity: 1,
                    price: (int)(dto.Amount * 1000)
                )
            };
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)(dto.Amount * 1000),
                description: description,
                items: items,
                returnUrl: dto.ReturnUrl,
                cancelUrl: dto.ReturnUrl
            );

            var paymentResult = await _payos.createPaymentLink(paymentData);
            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Amount = dto.Amount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderId = dto.OrderId,
                UserId = userId,
                PaymentMethod = "PayOS",
                Reference = orderCode.ToString(),
                Description = description,
                PaymentStatus = PaymentStatus.Pending
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

            await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
            _unitOfWork.Repository<Payment>().Update(payment);
            await _unitOfWork.SaveAsync();

            return transaction.TransactionStatus == TransactionStatus.Success;
        }
    }
}
