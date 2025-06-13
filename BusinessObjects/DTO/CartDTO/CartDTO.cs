using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.CartDTO
{
    public class CartDTO
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

        public string BuyerId { get; set; }

        public static implicit operator CartDTO(Cart v)
        {
            if (v == null) return null;
            return new CartDTO
            {
                Id = v.Id,
                CartNo = v.CartNo,
                Status = v.Status,
                Price = v.Price,
                Discount = v.Discount,
                Quantity = v.Quantity,
                CreatedAt = v.CreatedAt,
                UpdatedAt = v.UpdatedAt,
                IsDeleted = v.IsDeleted,
                ProductId = v.ProductId,
                BuyerId = v.BuyerId
            };
        }
    }
}
