using BusinessObjects.DTO.OrderDTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var orderNo = await _orderService.CreateOrderAsync(orderDto);
                return Ok(new { Message = "Đặt hàng thành công", OrderNo = orderNo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi tạo đơn hàng", Error = ex.Message });
            }
        }
    }
}
