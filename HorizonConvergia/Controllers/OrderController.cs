using BusinessObjects.DTO.OrderDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
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
        private readonly IUnitOfWork _unitOfWork;
        private string GetUserId() => User.FindFirst("UserId")?.Value ?? "";
        private string GetUserRole() => User.FindFirst("Role")?.Value ?? "";
        public OrdersController(IOrderService orderService, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
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
        [HttpPut("{id}/confirm")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> ConfirmOrder(string id)
        {
            var userId = GetUserId();

            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            if (order == null || order.IsDeleted)
                return NotFound("Đơn hàng không tồn tại.");

            if (order.SellerId != userId)
                return Forbid("Bạn không có quyền xác nhận đơn này.");

            var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Processing);
            return success ? Ok("Đã xác nhận đơn hàng.") : BadRequest("Không thể xác nhận.");
        }

        [HttpPut("{id}/process")]
        [Authorize(Policy = "Shipper")]
        public async Task<IActionResult> ProcessOrder(string id)
        {
            var userId = GetUserId();

            //var shipping = _unitOfWork.Repository<Shipping>()
            //    .Query().FirstOrDefault(s => s.OrderId == id && s.UserId == userId);

            //if (shipping == null)
            //    return Forbid("Bạn không được giao đơn hàng này.");

            var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Shipping);
            return success ? Ok("Đơn hàng đang được giao.") : BadRequest("Không thể cập nhật trạng thái.");
        }

        [HttpPut("{id}/deliver")]
        [Authorize(Policy = "Shipper")]
        public async Task<IActionResult> DeliverOrder(string id)
        {
            var userId = GetUserId();

            var shipping = _unitOfWork.Repository<Shipping>()
                .Query().FirstOrDefault(s => s.OrderId == id && s.CarrierId == userId);

            if (shipping == null)
                return Forbid("Bạn không được giao đơn hàng này.");

            var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Delivered);
            return success ? Ok("Đơn hàng đã giao thành công.") : BadRequest("Không thể cập nhật trạng thái.");
        }
    }
}

