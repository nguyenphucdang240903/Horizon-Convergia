using BusinessObjects.DTO.CategoryDTO;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync(string? name = null, int pageNumber = 1, int pageSize = 5);
        Task<CategoryDTO?> GetByIdAsync(string id);
        Task<IEnumerable<CategoryDTO>> GetSubCategoriesAsync(string parentId);
        Task<CategoryDTO> CreateAsync(CreateCategoryDTO categoryDto);
        Task<bool> UpdateAsync(string id, UpdateCategoryDTO categoryDto);
        Task<bool> DeleteAsync(string id);
    }
}
