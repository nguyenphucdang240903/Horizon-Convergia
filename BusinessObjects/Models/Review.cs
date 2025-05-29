namespace BusinessObjects.Models
{
    public class Review
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}
