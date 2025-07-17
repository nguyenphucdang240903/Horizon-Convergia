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
        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO categoryDto)
        {
            var cate = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = categoryDto.Name,
                ImageUrl = categoryDto.ImageUrl,
                ParentCategoryId = categoryDto.ParentCategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Repository<Category>().AddAsync(cate);
            await _unitOfWork.SaveAsync();
            return MapToDTO(cate);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null || category.IsDeleted)
            {
                return false; // Category not found or already deleted
            }
            category.IsDeleted = true; // Soft delete
            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync(string? name = null)
        {
            var query = _unitOfWork.Repository<Category>()
                .Query()
                .Where(c => c.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{name}%"));
            }

            var categories = await query.ToListAsync();
            return categories.Select(c => MapToDTO(c));
        }

        public async Task<CategoryDTO?> GetByIdAsync(string id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            return category == null || category.IsDeleted ? null : MapToDTO(category);
        }

        public async Task<bool> UpdateAsync(string id, UpdateCategoryDTO categoryDto)
        {
            var existing = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (existing == null || existing.IsDeleted)
            {
                return false;
            }

            existing.Name = categoryDto.Name;
            existing.ImageUrl = categoryDto.ImageUrl;
            existing.ParentCategoryId = categoryDto.ParentCategoryId;
            existing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Category>().Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<CategoryDTO>> GetSubCategoriesAsync(string parentId)
        {
            var subs = await _unitOfWork.Repository<Category>()
                .Query()
                .Where(c => c.ParentCategoryId == parentId && !c.IsDeleted)
                .ToListAsync();

            return subs.Select(MapToDTO);
        }

        private CategoryDTO MapToDTO(Category category) => new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            ParentCategoryId = category.ParentCategoryId
        };
    }
}
