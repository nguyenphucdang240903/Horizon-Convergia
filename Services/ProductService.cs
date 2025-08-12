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
            string? brand = null,
            string? model = null,
            int? year = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? description = null,
            string? location = null,
            string? condition = null,
            int? quantity = null,
            int? engineCapacity = null,
            string? fuelType = null,
            decimal? mileage = null,
            string? color = null,
            string? accessoryType = null,
            string? size = null,
            string? sparePartType = null,
            string? vehicleCompatible = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var query = _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.IsVerified && p.Status == ProductStatus.Active);

            query = ApplyProductFilters(query, categoryId, brand, model, year, minPrice, maxPrice,
                description, location, condition, quantity, engineCapacity, fuelType,
                mileage, color, accessoryType, size, sparePartType, vehicleCompatible);

            if (!string.IsNullOrEmpty(sortField))
            {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    "location" => ascending ? query.OrderBy(p => p.Location) : query.OrderByDescending(p => p.Location),
                    _ => query
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            var products = await query.ToListAsync();

            var result = new List<ProductDTO>();
            foreach (var p in products)
            {
                result.Add(await MapToDTOAsync(p));
            }
            return result;
        }
        public async Task<ProductDTO?> GetByIdAsync(string id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return product == null ? null : await MapToDTOAsync(product);
        }
        public async Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync(string sellerId,
            string? categoryId = null,
            string? brand = null,
            string? model = null,
            int? year = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? description = null,
            string? location = null,
            string? condition = null,
            int? quantity = null,
            int? engineCapacity = null,
            string? fuelType = null,
            decimal? mileage = null,
            string? color = null,
            string? accessoryType = null,
            string? size = null,
            string? sparePartType = null,
            string? vehicleCompatible = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(sellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted || seller.Role != UserRole.Seller)
                return Enumerable.Empty<ProductDTO>();

            var query = _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.SellerId == sellerId && !p.IsVerified && p.Status == ProductStatus.UnPaid_Seller);

            query = ApplyProductFilters(query, categoryId, brand, model, year, minPrice, maxPrice,
                description, location, condition, quantity, engineCapacity, fuelType,
                mileage, color, accessoryType, size, sparePartType, vehicleCompatible);

            if (!string.IsNullOrEmpty(sortField))
            {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    "location" => ascending ? query.OrderBy(p => p.Location) : query.OrderByDescending(p => p.Location),
                    _ => query
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var pagedProducts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ProductDTO>();
            foreach (var p in pagedProducts)
            {
                result.Add(await MapToDTOAsync(p));
            }

            return result;
        }
        public async Task<IEnumerable<ProductDTO>> GetUnpaidProductsAsync(string sellerId,
            string? categoryId = null,
            string? brand = null,
            string? model = null,
            int? year = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? description = null,
            string? location = null,
            string? condition = null,
            int? quantity = null,
            int? engineCapacity = null,
            string? fuelType = null,
            decimal? mileage = null,
            string? color = null,
            string? accessoryType = null,
            string? size = null,
            string? sparePartType = null,
            string? vehicleCompatible = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(sellerId);
            if (seller == null || !seller.IsVerified || seller.IsDeleted || seller.Role != UserRole.Seller)
                return Enumerable.Empty<ProductDTO>();

            var query = _unitOfWork.Repository<Product>()
                .Query()
                .Where(p => p.SellerId == sellerId && p.Status == ProductStatus.UnPaid_Seller);

            query = ApplyProductFilters(query, categoryId, brand, model, year, minPrice, maxPrice,
                description, location, condition, quantity, engineCapacity, fuelType,
                mileage, color, accessoryType, size, sparePartType, vehicleCompatible);

            if (!string.IsNullOrEmpty(sortField))
            {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    "location" => ascending ? query.OrderBy(p => p.Location) : query.OrderByDescending(p => p.Location),
                    _ => query
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var pagedProducts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ProductDTO>();
            foreach (var p in pagedProducts)
            {
                result.Add(await MapToDTOAsync(p));
            }

            return result;
        }

        public async Task<IEnumerable<ProductDTO>> GetFavoriteProductsAsync(
            string userId,
            string? categoryId = null,
            string? brand = null,
            string? model = null,
            int? year = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? description = null,
            string? location = null,
            string? condition = null,
            int? quantity = null,
            int? engineCapacity = null,
            string? fuelType = null,
            decimal? mileage = null,
            string? color = null,
            string? accessoryType = null,
            string? size = null,
            string? sparePartType = null,
            string? vehicleCompatible = null,
            string? sortField = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            if (user == null || !user.IsVerified || user.IsDeleted)
                return Enumerable.Empty<ProductDTO>();

            var baseQuery = _unitOfWork.Repository<FavoriteProduct>()
                .Query()
                .Where(f => f.UserId == userId)
                .Select(f => f.Product)
                .Where(p => p.IsVerified && p.Status == ProductStatus.Active);

            var query = ApplyProductFilters(baseQuery, categoryId, brand, model, year, minPrice, maxPrice,
                description, location, condition, quantity, engineCapacity, fuelType,
                mileage, color, accessoryType, size, sparePartType, vehicleCompatible);

            if (!string.IsNullOrEmpty(sortField))
            {
                query = sortField.ToLower() switch
                {
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "year" => ascending ? query.OrderBy(p => p.Year) : query.OrderByDescending(p => p.Year),
                    "createdat" => ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    "location" => ascending ? query.OrderBy(p => p.Location) : query.OrderByDescending(p => p.Location),
                    _ => query
                };
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var pagedProducts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ProductDTO>();
            foreach (var product in pagedProducts)
            {
                result.Add(await MapToDTOAsync(product));
            }

            return result;
        }


        public async Task<ProductCreateResult> CreateAsync(CreateProductDTO dto, string adminId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(adminId);
            if (user == null || !user.IsVerified || user.IsDeleted || user.Role != UserRole.Admin)
            {
                return new ProductCreateResult
                {
                    ErrorMessage = "Người dùng không hợp lệ hoặc không có quyền tạo sản phẩm."
                };
            }

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
                Status = ProductStatus.Active,
                IsVerified = true,
                SellerId = dto.SellerId,
                CategoryId = dto.CategoryId,
                CreatedAt = DateTime.UtcNow,

                EngineCapacity = dto.EngineCapacity,
                FuelType = dto.FuelType,
                Mileage = dto.Mileage,
                Color = dto.Color,
                AccessoryType = dto.AccessoryType,
                Size = dto.Size,
                SparePartType = dto.SparePartType,
                VehicleCompatible = dto.VehicleCompatible
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
                        Id = Guid.NewGuid().ToString(),
                        ImagesUrl = imageUrl,
                        ProductId = product.Id
                    };
                    await _unitOfWork.Repository<Images>().AddAsync(image);
                }
                await _unitOfWork.SaveAsync();
            }

            var productDto = await MapToDTOAsync(product);
            return new ProductCreateResult
            {
                Product = productDto
            };
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
                CreatedAt = DateTime.UtcNow,

                EngineCapacity = productDto.EngineCapacity,
                FuelType = productDto.FuelType,
                Mileage = productDto.Mileage,
                Color = productDto.Color,
                AccessoryType = productDto.AccessoryType,
                Size = productDto.Size,
                SparePartType = productDto.SparePartType,
                VehicleCompatible = productDto.VehicleCompatible
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

            var productDtoWithImages = await MapToDTOAsync(product);
            return new ProductCreateResult { Product = productDtoWithImages };
        }
        public async Task<string> SendPaymentLinkToSellerAsync(string productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (product.Status == ProductStatus.Active)
                throw new Exception("Sản phẩm đã được kích hoạt, không cần thanh toán.");
            var seller = await _unitOfWork.Repository<User>().GetByIdAsync(product.SellerId);

            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var returnUrl = "https://horizon-convergia.onrender.com/api/Payments/payos-callback";

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

        public async Task<bool> AddToFavoritesAsync(string userId, string productId)
        {
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            if (user == null || product == null) return false;

            var favRepo = _unitOfWork.Repository<FavoriteProduct>();
            bool alreadyExists = favRepo.Query().Any(f => f.UserId == userId && f.ProductId == productId);
            if (alreadyExists) return true;

            await favRepo.AddAsync(new FavoriteProduct
            {
                UserId = userId,
                ProductId = productId,
                CreateAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<bool> UpdateAsync(string id, UpdateProductDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var productRepo = _unitOfWork.Repository<Product>();
            var imageRepo = _unitOfWork.Repository<Images>();

            // Use the exposed Context to include Images
            var existing = await _unitOfWork.Context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null) return false;

            // Update product fields
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
            existing.EngineCapacity = dto.EngineCapacity;
            existing.FuelType = dto.FuelType;
            existing.Mileage = dto.Mileage;
            existing.Color = dto.Color;
            existing.AccessoryType = dto.AccessoryType;
            existing.Size = dto.Size;
            existing.SparePartType = dto.SparePartType;
            existing.VehicleCompatible = dto.VehicleCompatible;

            // Handle images
            var currentImages = existing.Images?.ToList() ?? new List<Images>();
            var newImageUrls = dto.ImageUrls?.Distinct().ToList() ?? new List<string>();

            // Remove old images
            var imagesToRemove = currentImages.Where(img => !newImageUrls.Contains(img.ImagesUrl)).ToList();
            foreach (var img in imagesToRemove)
            {
                imageRepo.Delete(img);
            }

            // Add new images
            foreach (var url in newImageUrls)
            {
                bool exists = currentImages.Any(img => img.ImagesUrl == url);
                if (!exists)
                {
                    var newImg = new Images
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImagesUrl = url,
                        ProductId = existing.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await imageRepo.AddAsync(newImg);
                }
            }

            productRepo.Update(existing);
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

        public async Task<bool> RemoveFromFavoritesAsync(string userId, string productId)
        {
            var favRepo = _unitOfWork.Repository<FavoriteProduct>();
            var favorite = favRepo.Query().FirstOrDefault(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null) return false;

            favRepo.Delete(favorite);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private async Task<ProductDTO> MapToDTOAsync(Product product)
        {
            var imageUrls = await _unitOfWork.Repository<Images>()
                .Query()
                .Where(img => img.ProductId == product.Id)
                .Select(img => img.ImagesUrl)
                .ToListAsync();

            return new ProductDTO
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
                CategoryId = product.CategoryId,
                EngineCapacity = product.EngineCapacity,
                FuelType = product.FuelType,
                Mileage = product.Mileage,
                Color = product.Color,
                AccessoryType = product.AccessoryType,
                Size = product.Size,
                SparePartType = product.SparePartType,
                VehicleCompatible = product.VehicleCompatible,
                ImageUrls = imageUrls
            };
        }

        private IQueryable<Product> ApplyProductFilters(
        IQueryable<Product> query,
        string? categoryId,
        string? brand,
        string? model,
        int? year,
        decimal? minPrice,
        decimal? maxPrice,
        string? description,
        string? location,
        string? condition,
        int? quantity,
        int? engineCapacity,
        string? fuelType,
        decimal? mileage,
        string? color,
        string? accessoryType,
        string? size,
        string? sparePartType,
        string? vehicleCompatible
        )
        {
            if (!string.IsNullOrEmpty(categoryId)) query = query.Where(p => p.CategoryId == categoryId);
            if (!string.IsNullOrEmpty(brand)) query = query.Where(p => p.Brand.Contains(brand));
            if (!string.IsNullOrEmpty(model)) query = query.Where(p => p.Model.Contains(model));
            if (year.HasValue) query = query.Where(p => p.Year == year.Value);
            if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);
            if (!string.IsNullOrEmpty(description)) query = query.Where(p => p.Description.Contains(description));
            if (!string.IsNullOrEmpty(location)) query = query.Where(p => p.Location.Contains(location));
            if (!string.IsNullOrEmpty(condition)) query = query.Where(p => p.Condition.Contains(condition));
            if (quantity.HasValue) query = query.Where(p => p.Quantity == quantity.Value);
            if (engineCapacity.HasValue) query = query.Where(p => p.EngineCapacity == engineCapacity.Value);
            if (!string.IsNullOrEmpty(fuelType)) query = query.Where(p => p.FuelType.Contains(fuelType));
            if (mileage.HasValue) query = query.Where(p => p.Mileage == mileage.Value);
            if (!string.IsNullOrEmpty(color)) query = query.Where(p => p.Color.Contains(color));
            if (!string.IsNullOrEmpty(accessoryType)) query = query.Where(p => p.AccessoryType.Contains(accessoryType));
            if (!string.IsNullOrEmpty(size)) query = query.Where(p => p.Size.Contains(size));
            if (!string.IsNullOrEmpty(sparePartType)) query = query.Where(p => p.SparePartType.Contains(sparePartType));
            if (!string.IsNullOrEmpty(vehicleCompatible)) query = query.Where(p => p.VehicleCompatible.Contains(vehicleCompatible));

            return query;
        }

    }


}
