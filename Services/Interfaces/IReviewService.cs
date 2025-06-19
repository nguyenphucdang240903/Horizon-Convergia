using BusinessObjects.DTO.ReviewDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllAsync();
        Task<ReviewDTO?> GetByIdAsync(string id);
        Task<ReviewDTO> CreateAsync(CreateReviewDTO dto);
        Task<bool> UpdateAsync(string id, UpdateReviewDTO dto);
        Task<bool> DeleteAsync(string id, DeleteReviewDTO dto);
    }
}
