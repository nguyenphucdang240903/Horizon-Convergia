using BusinessObjects.DTO.CartDTO;
using BusinessObjects.DTO.CategoryDTO;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<Cart> CreateCartAsync(string userId);
        Task<CartDetail> AddProductToCartAsync(string userId, string productId, int quantity);
        Task<List<CartDetail>> GetCartDetailsAsync(string cartId);
        Task<bool> RemoveCartDetailAsync(string cartDetailId);
        Task<bool> UpdateCartDetailQuantityAsync(string cartDetailId, int newQuantity);
    }
}
