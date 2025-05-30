using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetByIdAsync(long id);
        Task<ProductDTO> CreateAsync(CreateProductDTO productDto);
        Task<bool> UpdateAsync(long id, UpdateProductDTO productDto);
        Task<bool> DeleteAsync(long id);
    }


}
