using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.IsVerified)
                .ToListAsync();

            return products.Select(p => MapToDTO(p));
        }

        public async Task<ProductDTO?> GetByIdAsync(string id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return product == null ? null : MapToDTO(product);
        }

        public async Task<ProductDTO> CreateAsync(CreateProductDTO dto)
        {
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Price = dto.Price,
                Description = dto.Description,
                Location = dto.Location,
                Condition = dto.Condition,
                Quantity = dto.Quantity,
                Status = dto.Status,
                IsVerified = false,
                SellerId = dto.SellerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveAsync();
            return MapToDTO(product);
        }

        public async Task<bool> UpdateAsync(string id, UpdateProductDTO dto)
        {
            var existing = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (existing == null) return false;

            existing.Brand = dto.Brand;
            existing.Model = dto.Model;
            existing.Year = dto.Year;
            existing.Price = dto.Price;
            existing.Description = dto.Description;
            existing.Location = dto.Location;
            existing.Condition = dto.Condition;
            existing.Quantity = dto.Quantity;
            existing.Status = dto.Status;
            existing.IsVerified = dto.IsVerified;
            existing.SellerId = dto.SellerId;
            existing.CategoryId = dto.CategoryId;
            existing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Product>().Update(existing);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return false;

            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private ProductDTO MapToDTO(Product product) => new ProductDTO
        {
            Id = product.Id,
            Brand = product.Brand,
            Model = product.Model,
            Year = product.Year,
            Price = product.Price,
            Description = product.Description,
            Location = product.Location,
            Condition = product.Condition,
            Quantity = product.Quantity,
            Status = product.Status,
            IsVerified = product.IsVerified,
            CreatedAt = product.CreatedAt,
            SellerId = product.SellerId,
            CategoryId = product.CategoryId
        };

        private Product MapToEntity(ProductDTO dto) => new Product
        {
            Id = dto.Id,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Price = dto.Price,
            Description = dto.Description,
            Location = dto.Location,
            Condition = dto.Condition,
            Quantity = dto.Quantity,
            Status = dto.Status,
            IsVerified = dto.IsVerified,
            SellerId = dto.SellerId,
            CategoryId = dto.CategoryId
        };
    }


}
