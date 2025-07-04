﻿using BusinessObjects.DTO.ProductDTO;
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
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payos;

        public ProductService(IUnitOfWork unitOfWork, IOptions<PayOSSettings> options, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            var settings = options.Value;
            _payos = new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
            _emailService = emailService;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllAsync(
            string? categoryId = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var query = _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.IsVerified && p.Status == ProductStatus.Active);

            // Filter by Category
            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Sort by field
            if (!string.IsNullOrEmpty(sortField))
            {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    _ => query // No sorting if field is not recognized
                };
            }
            else
            {
                // Default sort by CreatedAt descending
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            // Pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var products = await query.ToListAsync();
            return products.Select(p => MapToDTO(p));
        }
        public async Task<ProductDTO?> GetByIdAsync(string id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return product == null ? null : MapToDTO(product);
        }
        public async Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync(string sellerId, 
            string? categoryId = null, 
            string? sortField = null, 
            bool ascending = true, 
            int pageNumber = 1, 
            int pageSize = 5)
        {
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(sellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted || seller.Role != UserRole.Seller)
            {
                return Enumerable.Empty<ProductDTO>();
            }

            var query = _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => !p.IsVerified && p.Status == ProductStatus.UnPaid_Seller && p.SellerId == sellerId);

            if(!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(sortField)) 
            { 
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    _ => query // No sorting if field is not recognized
                };
            }
            else
            {                 
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var products = await query.ToListAsync();
            return products.Select(p => MapToDTO(p));
        }
        public async Task<IEnumerable<ProductDTO>> GetUnpaidProductsAsync(string sellerId, 
            string? categoryId = null, 
            string? sortField = null, 
            bool ascending = true, 
            int pageNumber = 1, 
            int pageSize = 5)
        {
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(sellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted || seller.Role != UserRole.Seller)
            {
                return Enumerable.Empty<ProductDTO>();
            }

            var query =  _unitOfWork.Repository<Product>()
                 .Query()
                 .Where(p => p.SellerId == sellerId && p.Status == ProductStatus.UnPaid_Seller && p.SellerId == sellerId);

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(sortField)) {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    _ => query // No sorting if field is not recognized
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
            var products = await query.ToListAsync();
            return products.Select(p => MapToDTO(p));
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

            // Add images if any
            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                foreach (var imageUrl in dto.ImageUrls)
                {
                    var image = new Images
                    {
                        ImagesUrl = imageUrl,
                        ProductId = product.Id
                    };
                    await _unitOfWork.Repository<Images>().AddAsync(image);
                }
                await _unitOfWork.SaveAsync();
            }

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

            // Add images if any
            if (productDto.ImageUrls != null && productDto.ImageUrls.Any())
            {
                foreach (var imageUrl in productDto.ImageUrls)
                {
                    var image = new Images
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImagesUrl = imageUrl,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<Images>().AddAsync(image);
                }
                await _unitOfWork.SaveAsync();
            }

            return new ProductCreateResult { Product = product };
        }
        public async Task<string> SendPaymentLinkToSellerAsync(string productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product.Status == ProductStatus.Active)
                throw new Exception("Sản phẩm đã được kích hoạt, không cần thanh toán.");
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(product.SellerId);

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var returnUrl = "https://localhost:7076/api/Payments/payos-callback";

            var items = new List<ItemData>
    {
        new ItemData(
            name: "Thanh toán kích hoạt sản phẩm HorizonConvergia",
            quantity: product.Quantity,
            price: (int)product.Price
        )
    };

            var description = (product.Model ?? "HorizonConvergia").Substring(0, Math.Min(25, product.Model?.Length ?? 0));

            var paymentData = new PaymentData(
                orderCode: orderCode,
                amount: (int)((product.Price * product.Quantity) * (decimal)0.01m + 1_000m),
                description: description,
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
                ProductId = product.Id, 
                UserId = product.SellerId,
                PaymentMethod = "PayOS",
                Reference = orderCode.ToString(),
                Description = $"Thanh toán đăng bán sản phẩm: {product.Model}",
                PaymentStatus = PaymentStatus.Pending,
                PaymentType = PaymentType.SellerPayment,
                TransactionDate = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Payment>().AddAsync(payment);
            await _unitOfWork.SaveAsync();

            await _emailService.SendPaymentEmailAsync(seller.Email, paymentResult.checkoutUrl);

            return "Đã gửi link thanh toán tới email người bán.";
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
