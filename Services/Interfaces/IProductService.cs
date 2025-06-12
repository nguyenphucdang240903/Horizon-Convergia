using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(string id);
        Task<ProductDTO> CreateAsync(CreateProductDTO productDto);
        Task<bool> UpdateAsync(string id, UpdateProductDTO productDto);
        Task<bool> DeleteAsync(string id);
    }


}
