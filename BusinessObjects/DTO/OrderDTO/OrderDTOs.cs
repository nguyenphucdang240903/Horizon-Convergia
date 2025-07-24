using BusinessObjects.Enums;
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
        public List<CreateOrderFromCartSelectionDTO> OrderDetails { get; set; }
    }

    public class CreateOrderFromCartSelectionDTO
    {
        public List<string> ProductIds { get; set; }  
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
    }
        
    public class OrderSearchDTO
    {
        public string? BuyerId { get; set; }
        public string? SellerId { get; set; }
        public OrderStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public decimal? MinTotalPrice { get; set; }
        public decimal? MaxTotalPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class OrderListDTO
    {
        public string Id { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class OrderDetailDTO
    {
        public string Id { get; set; }
        public string OrderNo { get; set; }
        public decimal TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public decimal Discount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailItemDTO> OrderDetails { get; set; }
    }

    public class OrderDetailItemDTO
    {
        public string ProductId { get; set; }
        public string ProductType { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
