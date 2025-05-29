using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Order
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public long BuyerId { get; set; }
        public User Buyer { get; set; }

        public long SellerId { get; set; }
        public User Seller { get; set; }
        public Shipping Shipping { get; set; }
        public Payment Payment { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

    }

}
