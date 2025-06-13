using BusinessObjects.DTO.CartDTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var carts = await _cartService.GetAllAsync();
            return Ok(carts);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
            {
                return NotFound(new { message = "Cart not found." });
            }
            return Ok(cart);
        }

        [HttpPost("product/{productId}/user/{buyerId}")]
        public async Task<IActionResult> Create(string productId, string buyerId, [FromBody]CreateCartDTO value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _cartService.CreateAsync(productId, buyerId, value);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return BadRequest(new { message = result.ErrorMessage });
            }
            return CreatedAtAction(nameof(GetById), new { id = result.Cart.Id }, result.Cart);
        }

        // PUT api/<CartsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody]UpdateCartDTO value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var success = await _cartService.UpdateAsync(id, value);
            if (!success)
            {
                return NotFound(new { message = "Cart not found." });
            }
            return NoContent();
        }

        // DELETE api/<CartsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _cartService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Cart not found." });
            }
            return NoContent();
        }
    }
}
