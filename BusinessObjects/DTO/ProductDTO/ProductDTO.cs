﻿using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace BusinessObjects.DTO.ProductDTO
{
    public class ProductDTO
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Condition { get; set; }
        public int Quantity { get; set; }

        public int? EngineCapacity { get; set; }
        public string? FuelType { get; set; }
        public decimal? Mileage { get; set; }
        public string? Color { get; set; }
        public string? AccessoryType { get; set; }
        public string? Size { get; set; }
        public string? SparePartType { get; set; }
        public string? VehicleCompatible { get; set; }

        public ProductStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SellerId { get; set; }
        public string CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new();

        public static implicit operator ProductDTO(Product v)
        {
            if (v == null) return null;
            return new ProductDTO
            {
                Id = v.Id,
                Brand = v.Brand,
                Model = v.Model,
                Year = v.Year,
                Price = v.Price,
                Description = v.Description,
                Location = v.Location,
                Condition = v.Condition,
                Quantity = v.Quantity,
                EngineCapacity = v.EngineCapacity,
                FuelType = v.FuelType,
                AccessoryType = v.AccessoryType,
                Size = v.Size,
                Mileage = v.Mileage,
                Color = v.Color,
                SparePartType = v.SparePartType,
                VehicleCompatible = v.VehicleCompatible,
                Status = v.Status,
                IsVerified = v.IsVerified,
                CreatedAt = v.CreatedAt,
                SellerId = v.SellerId,
                CategoryId = v.CategoryId
            };
        }
    }


}
