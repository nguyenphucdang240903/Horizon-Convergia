using BusinessObjects.DTO.CartDTO;
using BusinessObjects.DTO.CategoryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartDTO>> GetAllAsync();
        Task<CartDTO?> GetByIdAsync(string id);
        Task<CartCreateResult> CreateAsync(string productId, string buyerId, CreateCartDTO categoryDto);
        Task<bool> UpdateAsync(string id, UpdateCartDTO categoryDto);
        Task<bool> DeleteAsync(string id);
    }
}
