using BusinessObjects.DTO.ProductDTO;
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
            [FromQuery] bool ascending = true)
        {
            var products = await _productService.GetAllAsync(categoryId, sortField, ascending);
            return Ok(products);
        }

        [HttpGet("unverified-unpaid")]
        public async Task<IActionResult> GetUnverifiedUnpaidProducts()
        {
            var products = await _productService.GetUnverifiedUnpaidProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _productService.CreateAsync(productDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{sellerId}")]
        public async Task<IActionResult> SellerCreate(string sellerId, [FromBody] CreateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _productService.SellerCreateAsync(sellerId, productDto);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Product.Id }, result.Product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _productService.UpdateAsync(id, productDto);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("verify/{id}")]
        public async Task<IActionResult> VerifyProduct(string id)
        {
            var result = await _productService.VerifyProduct(id);
            if (result == null)
                return NotFound();
            return Ok(new { message = "Xác minh sản phẩm thành công.", productId = result });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("send-payment-link/{productId}")]
        public async Task<IActionResult> SendPaymentLinkToSellerAsync(string productId, string returnUrl)
        {
            var link = await _productService.SendPaymentLinkToSellerAsync(productId, returnUrl);
            if (string.IsNullOrEmpty(link))
            {
                return BadRequest("Không gửi được link thanh toán.");
            }
            return Ok(link);
        }

        [HttpPut("activate/{productId}")]
        public async Task<IActionResult> ActivateProductAfterPaymentAsync(string productId)
        {
            var success = await _productService.ActivateProductAfterPaymentAsync(productId);
            return success ? NoContent() : NotFound();
        }
    }

}
