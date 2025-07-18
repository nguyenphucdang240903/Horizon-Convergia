using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Shipping
    {
        public string Id { get; set; }
        public string? CarrierId { get; set; }
        public string TrackingNumber { get; set; }
        public decimal ShippingFee { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public ShippingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }
        public User? User { get; set; }
    }

}
