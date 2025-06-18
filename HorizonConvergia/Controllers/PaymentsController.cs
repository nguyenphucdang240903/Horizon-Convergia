using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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


        [HttpPost("payos-callback")]
        public async Task<IActionResult> PayOSCallback([FromBody] PayOSReturnDTO dto)
        {
            var result = await _paymentService.HandlePayOSCallbackAsync(dto);
            return result ? Ok("Thanh toán thành công") : BadRequest("Thanh toán thất bại");
        }
    }

}
