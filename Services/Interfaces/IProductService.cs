﻿using BusinessObjects.DTO.ProductDTO;

namespace Services.Interfaces
{
    public interface IProductService
    {
        // GET
        Task<IEnumerable<ProductDTO>> GetAllAsync(
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
            int pageSize = 5);
        Task<ProductDTO?> GetByIdAsync(string id);
        Task<IEnumerable<ProductDTO>> GetUnverifiedUnpaidProductsAsync(string sellerId,
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
            int pageSize = 5);
        Task<IEnumerable<ProductDTO>> GetUnpaidProductsAsync(string sellerId,
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
            int pageSize = 5);
        Task<IEnumerable<ProductDTO>> GetFavoriteProductsAsync(string userId,
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
            int pageSize = 5);


        // POST
        Task<ProductCreateResult> CreateAsync(CreateProductDTO productDto, string adminId);
        Task<ProductCreateResult> SellerCreateAsync(string sellerId, CreateProductDTO productDto);
        Task<string> SendPaymentLinkToSellerAsync(string productId);
        Task<bool> AddToFavoritesAsync(string userId, string productId);
        // PUT
        Task<bool> UpdateAsync(string id, UpdateProductDTO productDto);


        // DELETE
        Task<bool> DeleteAsync(string id);
        Task<bool> RemoveFromFavoritesAsync(string userId, string productId);
    }

}