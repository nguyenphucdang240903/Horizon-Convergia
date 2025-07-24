namespace BusinessObjects.DTO.ProductDTO
{
    public class ProductFilterQuery
    {
        public string? CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Condition { get; set; }
        public int? Quantity { get; set; }
        public int? EngineCapacity { get; set; }
        public string? FuelType { get; set; }
        public decimal? Mileage { get; set; }
        public string? Color { get; set; }
        public string? AccessoryType { get; set; }
        public string? Size { get; set; }
        public string? SparePartType { get; set; }
        public string? VehicleCompatible { get; set; }

        public string? SortField { get; set; }
    }
}
