using BusinessObjects.DTO.ImageDTO;
using BusinessObjects.Models;
using DataAccessObjects;
using Services.Interfaces;

namespace Services
{
    public class ImagesService : IImagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ImagesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Images>> GetAllAsync()
        {
            return await _unitOfWork.Repository<Images>().GetAllAsync();
        }

        public async Task<Images> GetByIdAsync(string id)
        {
            return await _unitOfWork.Repository<Images>().GetByIdAsync(id);
        }

        public async Task<Images> CreateAsync(CreateImagesDTO dto)
        {
            var image = new Images
            {
                Id = Guid.NewGuid().ToString(),
                ImagesUrl = dto.ImagesUrl,
                ProductId = dto.ProductId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Images>().AddAsync(image);
            await _unitOfWork.SaveAsync();
            return image;
        }

        public async Task<bool> UpdateAsync(string id, UpdateImagesDTO dto)
        {
            var repo = _unitOfWork.Repository<Images>();
            var image = await repo.GetByIdAsync(id);
            if (image == null)
                return false;

            image.ImagesUrl = dto.ImagesUrl;
            repo.Update(image);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var repo = _unitOfWork.Repository<Images>();
            var image = await repo.GetByIdAsync(id);
            if (image == null)
                return false;

            repo.Delete(image);
            await _unitOfWork.SaveAsync();
            return true;
        }


    }
}
