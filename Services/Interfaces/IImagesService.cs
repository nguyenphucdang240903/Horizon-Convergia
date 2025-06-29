using BusinessObjects.DTO.ImageDTO;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IImagesService
    {
        Task<IEnumerable<Images>> GetAllAsync();
        Task<Images> GetByIdAsync(string id);
        Task<Images> CreateAsync(CreateImagesDTO dto);
        Task<bool> UpdateAsync(string id, UpdateImagesDTO dto);
        Task<bool> DeleteAsync(string id);
    }
}
