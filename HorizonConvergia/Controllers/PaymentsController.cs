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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingPayouts()
        {
            var payouts = await _unitOfWork.Repository<PayoutRequest>().Query()
                .Where(p => p.Status == PayoutStatus.Pending)
                .Include(p => p.User)
                .Select(p => new PayoutViewDTO
                {
                    PayoutRequestId = p.Id,
                    UserId = p.UserId,
                    FullName = p.User.Name,
                    BankName = p.User.BankName,
                    BankAccountNumber = p.User.BankAccountNumber,
                    Amount = p.Amount,
                    Reference = p.Reference,
                    CreatedAt = p.CreatedAt,
                    Status = p.Status
                }).ToListAsync();

            return Ok(payouts);
        }
        [HttpPost("approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePayout([FromBody] ApprovePayoutDTO dto)
        {
            var payout = _unitOfWork.Repository<PayoutRequest>().Query()
                .FirstOrDefault(p => p.Id == dto.PayoutRequestId && p.Status == PayoutStatus.Pending);

            if (payout == null) return NotFound("Payout not found or already processed.");

            payout.Status = dto.Approve ? PayoutStatus.Completed : PayoutStatus.Rejected;
            payout.ProcessedAt = DateTime.UtcNow;

            if (dto.Approve)
            {
                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(new PaymentTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = payout.UserId,
                    Amount = payout.Amount,
                    TransactionType = "Payout",
                    TransactionStatus = TransactionStatus.Success,
                    Reference = payout.Reference,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveAsync();
            return Ok("Payout updated.");
        }
    }

}
