namespace BusinessObjects.Models
{
    public class OrderDetail
    {
        public string Id { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public DateTime CreatedAt { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
