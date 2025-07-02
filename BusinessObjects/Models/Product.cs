using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Product
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
        public ProductStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Payment? Payment { get; set; }

        public string SellerId { get; set; }
        public User Seller { get; set; }
        public string CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Images> Images { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}
