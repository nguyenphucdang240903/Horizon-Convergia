using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System.Security.Claims;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentRequestDTO dto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token");

            string url;

            switch (dto.PaymentMethod.ToLower())
            {
                case "banktransfer":
                    url = await _paymentService.CreatePayOSUrlAsync(dto, userId);
                    break;
                default:
                    return BadRequest("Phương thức thanh toán không hợp lệ");
            }

            return Ok(new { Url = url });
        }


        [HttpGet("payos-callback")]
        public async Task<IActionResult> PayOSCallback([FromQuery] PayOSReturnDTO dto)
        {
            var result = await _paymentService.HandlePayOSCallbackAsync(dto);
            return result ? Ok("Thanh toán thành công") : BadRequest("Thanh toán thất bại");
        }

        [HttpGet("GetPendingPayouts")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var payouts = await _unitOfWork.Repository<Payment>().Query()
                .Where(p => (p.PaymentType == PaymentType.PayoutToSeller || p.PaymentType == PaymentType.PayoutToShipper)
                            && p.PaymentStatus == PaymentStatus.Pending)
                .Include(p => p.User)
                .Select(p => new PayoutViewDTO
                {
                    PaymentId = p.Id,
                    UserId = p.UserId,
                    FullName = p.User.Name,
                    BankName = p.User.BankName,
                    BankAccountNumber = p.User.BankAccountNumber,
                    BankAccountName = p.User.BankAccountName,
                    Amount = p.Amount,
                    Reference = p.Reference,
                    CreatedAt = p.CreatedAt,
                    Status = p.PaymentStatus
                }).ToListAsync();

            return Ok(payouts);
        }

        [HttpPost("approve")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> ApprovePayout([FromBody] ApprovePayoutDTO dto)
        {
            var payout = _unitOfWork.Repository<Payment>().Query()
                .FirstOrDefault(p => p.Id == dto.PaymentId && p.PaymentStatus == PaymentStatus.Pending);

            if (payout == null) return NotFound("Payout not found or already processed.");

            payout.PaymentStatus = dto.Approve ? PaymentStatus.Completed : PaymentStatus.Failed;
            payout.UpdatedAt = DateTime.UtcNow;

            if (dto.Approve)
            {
                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(new PaymentTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = payout.UserId,
                    PaymentId = payout.Id,
                    Amount = payout.Amount,
                    TransactionType = "ManualPayout",
                    TransactionStatus = TransactionStatus.Success,
                    Reference = payout.Reference,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _unitOfWork.Repository<Payment>().Update(payout);
            await _unitOfWork.SaveAsync();

            return Ok("Payout processed successfully.");
        }

    }

}
