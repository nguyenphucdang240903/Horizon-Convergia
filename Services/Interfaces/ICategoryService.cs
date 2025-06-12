using BusinessObjects.DTO.CategoryDTO;

namespace Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(long id);
        Task<CategoryDTO> CreateAsync(CategoryDTO categoryDto);
        Task<bool> UpdateAsync(long id, CategoryDTO categoryDto);
        Task<bool> DeleteAsync(long id);
    }
}
