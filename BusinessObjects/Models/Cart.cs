namespace BusinessObjects.Models
{
    public class Cart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BuyerId { get; set; }
        public User Buyer { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
