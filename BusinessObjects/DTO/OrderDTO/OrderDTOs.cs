using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.OrderDTO
{
    public class CreateOrderDTO
    {
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; }
    }

    public class CreateOrderDetailDTO
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string ProductType { get; set; }
    }
}
