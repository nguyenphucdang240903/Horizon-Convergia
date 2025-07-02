using BusinessObjects.DTO.OrderDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create-from-cart")]
        [Authorize(Policy = "Buyer")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrderFromCartDTO dto)
        {
            try
            {
                // Lấy BuyerId từ claim hoặc truyền vào DTO (tuỳ logic bạn đang dùng)
                var buyerId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                if (string.IsNullOrEmpty(buyerId))
                    return Unauthorized();

                var orderNos = await _orderService.CreateOrderAsync(dto, buyerId);
                return Ok(new { Message = "Order(s) created successfully.", OrderNumbers = orderNos });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] OrderSearchDTO searchDto)
        {
            var result = await _orderService.SearchOrdersAsync(searchDto);
            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetail(string orderId)
        {
            try
            {
                var order = await _orderService.GetOrderDetailAsync(orderId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
