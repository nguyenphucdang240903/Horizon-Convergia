﻿namespace BusinessObjects.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public string CartNo { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public string BuyerId { get; set; }
        public User Buyer { get; set; }
    }
}
