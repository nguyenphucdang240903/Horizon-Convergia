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
        public async Task<IActionResult> GetAll(
            [FromQuery] string? categoryId = null,
            [FromQuery] string? sortField = null,
            [FromQuery] bool ascending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            var products = await _productService.GetAllAsync(categoryId, sortField, ascending, pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("unverified-unpaid/{sellerId}")]
        public async Task<IActionResult> GetUnverifiedUnpaidProducts(string sellerId,
            [FromQuery] string? categoryId = null,
            [FromQuery] string? sortField = null,
            [FromQuery] bool ascending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            var products = await _productService.GetUnverifiedUnpaidProductsAsync(sellerId, categoryId, sortField, ascending, pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("unpaid/{sellerId}")]
        public async Task<IActionResult> GetUnpaidProducts(string sellerId,
            [FromQuery] string? categoryId = null,
            [FromQuery] string? sortField = null,
            [FromQuery] bool ascending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            var products = await _productService.GetUnpaidProductsAsync(sellerId, categoryId, sortField, ascending, pageNumber, pageSize);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
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


    }

}