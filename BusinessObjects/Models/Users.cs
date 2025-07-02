using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
        public DateTime? Dob { get; set; }
        public string? ShopName { get; set; }
        public string? shopDescription { get; set; }
        public string? BusinessType { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerificationTokenExpires { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpires { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public Cart? Cart { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public ICollection<Shipping> Shippings { get; set; }
        public ICollection<Order> OrdersAsBuyer { get; set; }
        public ICollection<Order> OrdersAsSeller { get; set; }

    }
}
