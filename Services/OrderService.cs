using BusinessObjects.DTO.OrderDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            var orderId = Guid.NewGuid().ToString();
            var order = new Order
            {
                Id = orderId,
                OrderNo = $"ORD-{DateTime.UtcNow.Ticks}",
                BuyerId = orderDto.BuyerId,
                SellerId = orderDto.SellerId,
                ShippingAddress = orderDto.ShippingAddress,
                Discount = orderDto.Discount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                IsDeleted = false,
                TotalPrice = orderDto.OrderDetails.Sum(od => od.Price - od.Discount),
                OrderDetails = orderDto.OrderDetails.Select(od => new OrderDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = od.ProductId,
                    Price = od.Price,
                    Discount = od.Discount,
                    ProductType = od.ProductType,
                    CreatedAt = DateTime.UtcNow,
                    OrderId = orderId
                }).ToList()
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveAsync();
            return order.OrderNo;
        }
    }

}
