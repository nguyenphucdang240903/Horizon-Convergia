using BusinessObjects.DTO.CategoryDTO;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync(string? name = null);
        Task<CategoryDTO?> GetByIdAsync(string id);
        Task<CategoryDTO> CreateAsync(CreateCategoryDTO categoryDto);
        Task<bool> UpdateAsync(string id, UpdateCategoryDTO categoryDto);
        Task<bool> DeleteAsync(string id);
    }
}
