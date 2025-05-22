using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class OrderDetail
    {
        public long Id { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public DateTime CreatedAt { get; set; }

        public long OrderId { get; set; }
        public Order Order { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }
    }
}
