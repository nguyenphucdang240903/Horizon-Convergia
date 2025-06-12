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
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDTO productDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var success = await _productService.UpdateAsync(id, productDto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _productService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }


}
