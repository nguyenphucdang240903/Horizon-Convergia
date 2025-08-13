using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.CartDTO
{
    public class CartDto
    {
        public string Id { get; set; }
        public string BuyerId { get; set; }
        public List<CartItemDto> Items { get; set; }
    }

    public class CartItemDto
    {
        public string ProductId { get; set; }
        public string ProductBrand { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } 
    }

    public class CartDetailDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductBrand { get; set; }
        public string ProductModel { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
