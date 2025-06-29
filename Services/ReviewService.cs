using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.DTO.ReviewDTO;
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
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllAsync()
        {
            var reviews = await _unitOfWork.Repository<Review>().GetAllAsync();
            return reviews.Select(r => MapToDTO(r));
        }
        public async Task<ReviewDTO?> GetByIdAsync(string id)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
            return review == null ? null : MapToDTO(review);
        }

        public async Task<ReviewDTO> CreateAsync(CreateReviewDTO dto)
        {
            var review = new Review
            {
                Id = Guid.NewGuid().ToString(),
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = dto.IsDeleted,
                ProductId = dto.ProductId,
                UserId = dto.UserId,
            };

            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.SaveAsync();
            return MapToDTO(review);
        }
        public async Task<bool> UpdateAsync(string id, UpdateReviewDTO dto)
        {
            var existing = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
            if (existing == null) return false;

            existing.Comment = dto.Comment;
            existing.Rating = dto.Rating;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.IsDeleted = dto.IsDeleted;
            existing.ProductId = dto.ProductId;
            existing.UserId = dto.UserId;

            _unitOfWork.Repository<Review>().Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(string id, DeleteReviewDTO dto)
        {
            var existing = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsDeleted = true;

            _unitOfWork.Repository<Review>().Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }
        private ReviewDTO MapToDTO(Review review ) => new ReviewDTO
        {
            Id = review.Id,
            Comment = review.Comment,
            Rating = review.Rating,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = review.IsDeleted,
            ProductId = review.ProductId,
            UserId = review.UserId,
        };
    }
}
