using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.Models;
using DataAccessObjects;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BlogDTO>> GetAllAsync()
        {
            var blogs = await _unitOfWork.Repository<Blog>().GetAllAsync();
            return blogs.Select(b => MapToDTO(b));
        }
        public async Task<BlogDTO?> GetByIdAsync(string id)
        {
            var blog = await _unitOfWork.Repository<Blog>().GetByIdAsync(id);
            return blog == null ? null : MapToDTO(blog);
        }

        public async Task<BlogDTO> CreateAsync(CreateBlogDTO dto)
        {
            var blog = new Blog
            {
                Id = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                IsDeleted = dto.IsDeleted,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.CreatedAt
            };

            await _unitOfWork.Repository<Blog>().AddAsync(blog);
            await _unitOfWork.SaveAsync();
            return MapToDTO(blog);
        }

        public async Task<bool> UpdateAsync(string id, UpdateBlogDTO dto)
        {
            var existing = await _unitOfWork.Repository<Blog>().GetByIdAsync(id);
            if (existing == null) return false;

            existing.Title = dto.Title;
            existing.Content = dto.Content;
            existing.ImageUrl = dto.ImageUrl;
            existing.IsDeleted = dto.IsDeleted;
            existing.AuthorId = dto.AuthorId;
            existing.CategoryId = dto.CategoryId;
            existing.UpdatedAt = dto.UpdatedAt;

            _unitOfWork.Repository<Blog>().Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var blog = await _unitOfWork.Repository<Blog>().GetByIdAsync(id);
            if (blog == null) return false;

            blog.IsDeleted = true;
            _unitOfWork.Repository<Blog>().Update(blog);
            await _unitOfWork.SaveAsync();
            return true;
        }
        private BlogDTO MapToDTO(Blog blog) => new BlogDTO
        {
            Id = blog.Id,
            Title = blog.Title,
            Content = blog.Content,
            ImageUrl = blog.ImageUrl,
            IsDeleted = blog.IsDeleted,
            AuthorId = blog.AuthorId,
            CategoryId = blog.CategoryId,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt
        };

    }
}
