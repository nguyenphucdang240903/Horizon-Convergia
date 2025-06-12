using BusinessObjects.DTO.CategoryDTO;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryDTO> CreateAsync(CategoryDTO categoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _unitOfWork.Repository<Category>()
                .Query()
                .Where(c => c.IsDeleted == false)
                .ToListAsync();

            return categories.Select(c => MapToDTO(c));
        }

        public Task<CategoryDTO?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(long id, CategoryDTO categoryDto)
        {
            throw new NotImplementedException();
        }

        private CategoryDTO MapToDTO(Category category) => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
