﻿using BusinessObjects.DTO.CartDTO;
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

        [HttpPost("{userId}/add/{productId}")]
        public async Task<IActionResult> AddToCart(string userId, string productId, [FromQuery] int quantity)
        {
            var result = await _cartService.AddProductToCartAsync(userId, productId, quantity);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUser(string userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            return cart == null ? NotFound() : Ok(cart);
        }

        [HttpGet("{cartId}/details")]
        public async Task<IActionResult> GetCartDetails(string cartId)
        {
            var details = await _cartService.GetCartDetailsAsync(cartId);
            return Ok(details);
        }

        [HttpDelete("detail/{cartDetailId}")]
        public async Task<IActionResult> RemoveCartDetail(string cartDetailId)
        {
            var result = await _cartService.RemoveCartDetailAsync(cartDetailId);
            return result ? Ok() : NotFound();
        }

        [HttpPut("detail/{cartDetailId}/quantity/{newQuantity}")]
        public async Task<IActionResult> UpdateCartDetailQuantity(string cartDetailId, int newQuantity)
        {
            var result = await _cartService.UpdateCartDetailQuantityAsync(cartDetailId, newQuantity);
            return result ? Ok() : NotFound();
        }
    }
}
