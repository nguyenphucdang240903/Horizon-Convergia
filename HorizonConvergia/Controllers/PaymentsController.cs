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

        [HttpPost("multi-payment")]
        public async Task<IActionResult> CreateMultiOrderPayment(CreatePaymentRequestDTO dto)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("UserId not found in token");

            if (dto.OrderIds == null || !dto.OrderIds.Any())
                return BadRequest("OrderIds cannot be empty");

            string url;

            switch (dto.PaymentMethod.ToLower())
            {
                case "payos":
                    url = await _paymentService.CreateMultiOrderPayOSUrlAsync(dto, userId);
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

            string title = result ? "Thanh toán thành công!" : "Thanh toán thất bại!";
            string message = result
                ? "Cảm ơn bạn đã thanh toán. Giao dịch của bạn đã được xử lý thành công."
                : "Rất tiếc! Đã xảy ra lỗi trong quá trình thanh toán. Vui lòng thử lại sau.";
            string iconUrl = result
                ? "https://cdn-icons-png.flaticon.com/512/845/845646.png"  // success
                : "https://cdn-icons-png.flaticon.com/512/463/463612.png"; // failed
            string color = result ? "#22c55e" : "#ef4444";

            string html = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{title}</title>
    <link href='https://fonts.googleapis.com/css2?family=Poppins:wght@400;600&display=swap' rel='stylesheet'>
    <style>
        body {{
            margin: 0;
            font-family: 'Poppins', sans-serif;
            background: linear-gradient(to right, #f0f9ff, #e0f2fe);
            color: #1e293b;
        }}
        .container {{
            max-width: 640px;
            margin: 60px auto;
            background: #fff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.08);
        }}
        .header {{
            text-align: center;
            padding: 24px;
            background: #f8fafc;
        }}
        .logo {{
            width: 64px;
            margin-bottom: 12px;
        }}
        .title {{
            font-size: 24px;
            font-weight: 600;
            color: {color};
        }}
        .status {{
            text-align: center;
            padding: 32px 24px;
        }}
        .status img {{
            width: 80px;
            margin-bottom: 24px;
        }}
        .status p {{
            font-size: 16px;
            line-height: 1.6;
            color: #334155;
        }}
        .buttons {{
            margin-top: 32px;
            display: flex;
            justify-content: center;
            gap: 16px;
            flex-wrap: wrap;
        }}
        .button {{
            background: #0ea5e9;
            color: #fff;
            padding: 12px 24px;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 500;
            transition: background 0.3s ease;
        }}
        .button:hover {{
            background: #0284c7;
        }}
        .footer {{
            text-align: center;
            background: #f1f5f9;
            padding: 16px;
            font-size: 14px;
            color: #64748b;
        }}
        @media (max-width: 480px) {{
            .title {{
                font-size: 20px;
            }}
            .button {{
                padding: 10px 18px;
                font-size: 14px;
            }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <img src='https://www.horizonconvergia.click/assets/logo-Cw2ulTmz.png' class='logo' alt='Horizon Logo' />
            <div class='title'>{title}</div>
        </div>
        <div class='status'>
            <img src='{iconUrl}' alt='Status Icon' />
            <p>{message}</p>
            <div class='buttons'>
                <a href='https://horizon-convergia.click' class='button'>Về trang chủ</a>
                <a href='https://horizon-convergia.click/user/orders' class='button'>Xem đơn hàng</a>
            </div>
        </div>
        <div class='footer'>
            Horizon Convergia - FPT University<br />
            Email này được gửi từ hệ thống tự động.
        </div>
    </div>
</body>
</html>";

            return Content(html, "text/html");
        }


        [HttpGet("GetPendingPayouts")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetPendingPayouts([FromQuery] PayoutFilterDTO filter)
        {
            var query = _unitOfWork.Repository<Payment>().Query()
                .Include(p => p.User)
                .Where(p => (p.PaymentType == PaymentType.PayoutToSeller || p.PaymentType == PaymentType.PayoutToShipper)
                            && p.PaymentStatus == PaymentStatus.Pending);
                

            // Filtering
            if (!string.IsNullOrWhiteSpace(filter.FullName))
                query = query.Where(p => p.User.Name.ToLower().Contains(filter.FullName.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.BankName))
                query = query.Where(p => p.User.BankName.ToLower().Contains(filter.BankName.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.BankAccountNumber))
                query = query.Where(p => p.User.BankAccountNumber.Contains(filter.BankAccountNumber));

            if (!string.IsNullOrWhiteSpace(filter.BankAccountName))
                query = query.Where(p => p.User.BankAccountName.ToLower().Contains(filter.BankAccountName.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Reference))
                query = query.Where(p => p.Reference.ToLower().Contains(filter.Reference.ToLower()));

            if (filter.FromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(p => p.CreatedAt <= filter.ToDate.Value);

            // Pagination
            var totalItems = await query.CountAsync();
            var payouts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
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
                })
                .ToListAsync();

            var result = new
            {
                filter.Page,
                filter.PageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize),
                Data = payouts
            };

            return Ok(result);
        }


        [HttpPost("ApprovePayOut")]
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
