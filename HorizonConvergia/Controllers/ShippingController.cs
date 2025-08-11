using BusinessObjects.DTO.ShippingDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Admin")]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpGet("unassigned-orders")]
        public async Task<IActionResult> GetUnassignedOrders(
            [FromQuery] string? buyerName,
            [FromQuery] string? sellerName,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var filter = new UnassignedOrdersFilterRequest
            {
                BuyerName = buyerName,
                SellerName = sellerName,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _shippingService.GetUnassignedOrdersAsync(filter);

            return Ok(new
            {
                IsSuccess = true,
                Message = "Lấy danh sách đơn hàng chưa gán shipper thành công.",
                Data = result
            });
        }

        [HttpPost("assign-shipper")]
        public async Task<IActionResult> AssignShipper([FromBody] AssignShipperRequest request)
        {
            var result = await _shippingService.AssignShipperAsync(request.OrderId, request.CarrierId);
            return Ok(new
            {
                IsSuccess = true,
                Message = "Chọn shipper thành công.",
                Data = result
            });
        }
    }
}
