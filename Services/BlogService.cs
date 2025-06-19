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
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AuthorId = dto.AuthorId,
            };

            await _unitOfWork.Repository<Blog>().AddAsync(blog);
            await _unitOfWork.SaveAsync();
            return MapToDTO(blog);
        }
        public async Task<bool> UpdateAsync(string id, UpdateBlogDTO dto)
        {
            var existing = await _unitOfWork.Repository<Blog>().GetByIdAsync(id);
            if (existing == null) return false;

            existing.Content = dto.Content;
            existing.ImageUrl = dto.ImageUrl;
            existing.AuthorId = dto.AuthorId;

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
            Content = blog.Content,
            ImageUrl = blog.ImageUrl,
            AuthorId = blog.AuthorId
        };
    }
}
