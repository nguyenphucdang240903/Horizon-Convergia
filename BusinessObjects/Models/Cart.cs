using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Cart
    {
        public long Id { get; set; }
        public string CartNo { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

        public long BuyerId { get; set; }
        public User Buyer { get; set; }
    }
}
