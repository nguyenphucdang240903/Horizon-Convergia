using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.DTO.ProductDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterQuery filter, 
            [FromQuery] bool Ascending = true, 
            [FromQuery] int PageNumber =1, [FromQuery] int PageSize = 5)
        {
            var products = await _productService.GetAllAsync(
                filter.CategoryId, filter.Brand, filter.Model, filter.Year, filter.MinPrice, filter.MaxPrice,
                filter.Description, filter.Location, filter.Condition, filter.Quantity, filter.EngineCapacity,
                filter.FuelType, filter.Mileage, filter.Color, filter.AccessoryType, filter.Size,
                filter.SparePartType, filter.VehicleCompatible,
                filter.SortField, Ascending, PageNumber, PageSize);

            return Ok(products);
        }


        [HttpGet("unverified-unpaid/{sellerId}")]
        public async Task<IActionResult> GetUnverifiedUnpaidProducts(string sellerId, [FromQuery] ProductFilterQuery filter,
            [FromQuery] bool Ascending = true,
            [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5)
        {
            var products = await _productService.GetUnverifiedUnpaidProductsAsync(
                sellerId, filter.CategoryId, filter.Brand, filter.Model, filter.Year, filter.MinPrice, filter.MaxPrice,
                filter.Description, filter.Location, filter.Condition, filter.Quantity, filter.EngineCapacity,
                filter.FuelType, filter.Mileage, filter.Color, filter.AccessoryType, filter.Size,
                filter.SparePartType, filter.VehicleCompatible,
                filter.SortField, Ascending, PageNumber, PageSize);

            return Ok(products);
        }

        [HttpGet("unpaid/{sellerId}")]
        public async Task<IActionResult> GetUnpaidProducts(string sellerId, [FromQuery] ProductFilterQuery filter,
            [FromQuery] bool Ascending = true,
            [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5)
        {
            var products = await _productService.GetUnpaidProductsAsync(
                sellerId, filter.CategoryId, filter.Brand, filter.Model, filter.Year, filter.MinPrice, filter.MaxPrice,
                filter.Description, filter.Location, filter.Condition, filter.Quantity, filter.EngineCapacity,
                filter.FuelType, filter.Mileage, filter.Color, filter.AccessoryType, filter.Size,
                filter.SparePartType, filter.VehicleCompatible,
                filter.SortField, Ascending, PageNumber, PageSize);

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }
        [HttpGet("favorite/{userId}")]
        public async Task<IActionResult> GetFavorites(string userId, [FromQuery] ProductFilterQuery filter,
            [FromQuery] bool Ascending = true,
            [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5)
        {
            var favorites = await _productService.GetFavoriteProductsAsync(
                userId, filter.CategoryId, filter.Brand, filter.Model, filter.Year, filter.MinPrice, filter.MaxPrice,
                filter.Description, filter.Location, filter.Condition, filter.Quantity, filter.EngineCapacity,
                filter.FuelType, filter.Mileage, filter.Color, filter.AccessoryType, filter.Size,
                filter.SparePartType, filter.VehicleCompatible,
                filter.SortField, Ascending, PageNumber, PageSize);

            return Ok(favorites);
        }


        [HttpPost("admin/{adminId}")]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO productDto, string adminId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _productService.CreateAsync(productDto, adminId);
            return CreatedAtAction(nameof(GetById), new { id = created.Product.Id }, created.Product);
        }

        [HttpPost("seller/{sellerId}")]
        public async Task<IActionResult> SellerCreate(string sellerId, [FromBody] CreateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _productService.SellerCreateAsync(sellerId, productDto);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Product.Id }, result.Product);
        }


        [HttpPost("send-payment-link")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SendPaymentLinkToSellerAsync([FromBody] SendPaymentLinkDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ProductId))
                return BadRequest("Thiếu thông tin sản phẩm.");

            var result = await _productService.SendPaymentLinkToSellerAsync(dto.ProductId);

            if (string.IsNullOrWhiteSpace(result))
                return BadRequest("Không gửi được link thanh toán.");

            return Ok(new { message = "Đã gửi link thanh toán", url = result });
        }

        [HttpPost("{productId}/favorite/{userId}")]
        public async Task<IActionResult> AddToFavorites(string userId, string productId)
        {
            var success = await _productService.AddToFavoritesAsync(userId, productId);
            return success ? Ok(new { message = "Added to favorites." }) : NotFound();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _productService.UpdateAsync(id, productDto);
            return success ? NoContent() : NotFound();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{productId}/favorive/{userId}")]
        public async Task<IActionResult> RemoveFromFavorites(string userId, string productId)
        {
            var success = await _productService.RemoveFromFavoritesAsync(userId, productId);
            return success ? Ok(new { message = "Removed from favorites." }) : NotFound();
        }

    }

}