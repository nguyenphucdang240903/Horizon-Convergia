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
        public string CategoryId { get; set; }
        //public List<string> ImageUrls { get; set; } = new();
    }
}
