using BusinessObjects.Enums;

namespace BusinessObjects.DTO.ProductDTO
{
    public class ProductDTO
    {
        public long Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Condition { get; set; }
        public int Quantity { get; set; }
        public ProductStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public long SellerId { get; set; }
        public long CategoryId { get; set; }
    }

}
