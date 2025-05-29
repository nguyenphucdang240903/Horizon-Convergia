using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Payment
    {
        public long Id { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long OrderId { get; set; }
        public Order Order { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }
    }
}
