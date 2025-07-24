using BusinessObjects.DTO.BlogDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDTO>> GetAllAsync();
        Task<BlogDTO?> GetByIdAsync(string id);
        //Task<BlogDTO> CreateAsync(CreateBlogDTO dto);
        Task<bool> UpdateAsync(string id, UpdateBlogDTO dto);
        Task<bool> DeleteAsync(string id);
        //Task<IEnumerable<BlogDTO>> GetByCategoryAsync(string categoryId);
        Task<IEnumerable<BlogDTO>> CreateMultipleAsync(CreateBlogDTO dto);
    }
}
