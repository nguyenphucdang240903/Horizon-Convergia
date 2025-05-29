using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class PaymentTransaction
    {
        public long Id { get; set; }
        public long PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
        public Payment Payment { get; set; }

    }
}
