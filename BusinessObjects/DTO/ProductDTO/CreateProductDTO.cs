using BusinessObjects.Enums;

namespace BusinessObjects.DTO.ProductDTO
{
    public class CreateProductDTO
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Condition { get; set; }
        public int Quantity { get; set; }

        public int? EngineCapacity { get; set; } = null;
        public string? FuelType { get; set; } = null;
        public decimal? Mileage { get; set; } = null;
        public string? Color { get; set; } = null;
        public string? AccessoryType { get; set; } = null;
        public string? Size { get; set; } = null;
        public string? SparePartType { get; set; } = null;
        public string? VehicleCompatible { get; set; } = null;

        public string SellerId { get; set; }
        public string CategoryId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

    public class UpdateProductDTO
    {
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

        public string CategoryId { get; set; }
        //public List<string> ImageUrls { get; set; } = new();
    }
}
