using BusinessObjects.DTO.PaymentDTO;
using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using DataAccessObjects.Setting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Services.Interfaces;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payos;

        public ProductService(IUnitOfWork unitOfWork, IOptions<PayOSSettings> options)
        {
            _unitOfWork = unitOfWork;
            var settings = options.Value;
            _payos = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync()
        {
            var products = await _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.IsVerified && p.Status == ProductStatus.Active)
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
                Status = ProductStatus.UnPaid_Seller,
                IsVerified = false,
                SellerId = dto.SellerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveAsync();
            return MapToDTO(product);
        }

        public async Task<ProductCreateResult?> SellerCreateAsync(string sellerId, CreateProductDTO productDto)
        {
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(sellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted)
                return new ProductCreateResult { ErrorMessage = "Không tìm thấy người bán." };

            if (seller.Role != UserRole.Seller)
                return new ProductCreateResult { ErrorMessage = "Người dùng này không có vai trò Seller." };

            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Brand = productDto.Brand,
                Model = productDto.Model,
                Year = productDto.Year,
                Price = productDto.Price,
                Description = productDto.Description,
                Location = productDto.Location,
                Condition = productDto.Condition,
                Quantity = productDto.Quantity,
                Status = ProductStatus.UnPaid_Seller,
                IsVerified = false,
                SellerId = sellerId,
                CategoryId = productDto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveAsync();
            return new ProductCreateResult { Product = product };

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

        public async Task<string> VerifyProduct(string id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return "Không tìm thấy sản phẩm.";
            }
            if (product.IsVerified)
            {
                return "Sản phẩm đã được xác minh.";
            }
            product.IsVerified = true;
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveAsync();
            return "Xác minh sản phẩm thành công.";
        }

        public async Task<string> SendPaymentLinkToSellerAsync(string productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product == null || !product.IsVerified || product.Status != ProductStatus.UnPaid_Seller)
                return "Không tìm thấy sản phẩm hợp lệ.";

            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(product.SellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted || seller.Role != UserRole.Seller)
                return "Không tìm thấy người bán hợp lệ.";

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var items = new List<ItemData>
    {
        new ItemData(
            name: "Thanh toán sản phẩm HorizonConvergia",
            quantity: product.Quantity,
            price: (int)(product.Price)
        )
    };
            var returnUrl = "https://localhost:7076/api/Payments/payos-callback";
            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)(product.Price * product.Quantity),
                description: (product.Model ?? "HorizonConvergia").Substring(0, Math.Min(25, product.Model?.Length ?? 0)),
                items: items,
                returnUrl: returnUrl,
                cancelUrl: returnUrl
            );

            var paymentResult = await _payos.createPaymentLink(paymentData);

            var payment = new Payment
            {
                Id = Guid.NewGuid().ToString(),
                Amount = product.Price * product.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                OrderId = product.Id, 
                UserId = product.SellerId,
                PaymentMethod = "PayOS",
                Reference = orderCode.ToString(),
                Description = $"Thanh toán đăng bán sản phẩm: {product.Model}",
                PaymentStatus = PaymentStatus.Pending
            };

            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            await _unitOfWork.SaveAsync();

            var emailService = new EmailService();
            await emailService.SendPaymentEmailAsync(seller.Email, paymentResult.checkoutUrl);

            return "Đã gửi link thanh toán tới email người bán.";
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
