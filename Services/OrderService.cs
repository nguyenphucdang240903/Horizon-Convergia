using BusinessObjects.DTO.OrderDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<string>> CreateOrderAsync(CreateOrderFromCartDTO dto, string buyerId)
        {
            var carts = _unitOfWork.Repository<Cart>().Query()
                .Where(c => dto.CartId.Contains(c.Id) && c.BuyerId == buyerId && !c.IsDeleted)
                .ToList();

            if (carts == null || !carts.Any())
                throw new Exception("No valid cart items found.");

            // Lấy tất cả sản phẩm liên quan trong giỏ 1 lần, tránh truy vấn lặp
            var productIds = carts.Select(c => c.ProductId).Distinct().ToList();
            var products = _unitOfWork.Repository<Product>()
                .Query()
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            // Kiểm tra tồn kho và trạng thái từng sản phẩm
            foreach (var cart in carts)
            {
                var product = products.FirstOrDefault(p => p.Id == cart.ProductId);

                if (product == null || !product.IsVerified || product.Status != ProductStatus.Active)
                    throw new Exception($"Product {cart.ProductId} is unavailable.");

                if (cart.Quantity > product.Quantity)
                    throw new Exception($"Product {product.Model} only has {product.Quantity} item(s) left in stock.");
            }

            // Nhóm theo SellerId
            var groupedCarts = carts.GroupBy(c => products.First(p => p.Id == c.ProductId).SellerId);
            var orderNumbers = new List<string>();

            foreach (var sellerGroup in groupedCarts)
            {
                var orderId = Guid.NewGuid().ToString();
                var firstProduct = products.First(p => p.Id == sellerGroup.First().ProductId);

                var order = new Order
                {
                    Id = orderId,
                    OrderNo = $"ORD-{DateTime.UtcNow.Ticks}-{sellerGroup.Key.Substring(0, 4)}",
                    BuyerId = buyerId,
                    SellerId = sellerGroup.Key,
                    ShippingAddress = dto.ShippingAddress,
                    Discount = dto.Discount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    IsDeleted = false,
                    TotalPrice = sellerGroup.Sum(c => (c.Price - c.Discount) * c.Quantity),
                    OrderDetails = sellerGroup.Select(c =>
                    {
                        var product = products.First(p => p.Id == c.ProductId);
                        return new OrderDetail
                        {
                            Id = Guid.NewGuid().ToString(),
                            ProductId = c.ProductId,
                            Price = c.Price,
                            Discount = c.Discount,
                            ProductType = product.Category?.Name ?? "Khác",
                            CreatedAt = DateTime.UtcNow,
                            OrderId = orderId
                        };
                    }).ToList()
                };

                await _unitOfWork.Repository<Order>().AddAsync(order);

                // Trừ tồn kho
                foreach (var cart in sellerGroup)
                {
                    var product = products.First(p => p.Id == cart.ProductId);
                    product.Quantity -= cart.Quantity;
                    if (product.Quantity < 0)
                        product.Quantity = 0;
                }

                orderNumbers.Add(order.OrderNo);
            }

            // Xóa giỏ hàng
            foreach (var cart in carts)
            {
                _unitOfWork.Repository<Cart>().Delete(cart);
            }

            await _unitOfWork.SaveAsync();
            return orderNumbers;
        }


        public async Task<OrderDetailDTO> GetOrderDetailAsync(string orderId)
        {
            var order = await _unitOfWork.Repository<Order>()
                .Query()
                .Where(o => o.Id == orderId && !o.IsDeleted)
                .Select(o => new OrderDetailDTO
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    TotalPrice = o.TotalPrice,
                    ShippingAddress = o.ShippingAddress,
                    Discount = o.Discount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemDTO
                    {
                        ProductId = od.ProductId,
                        ProductType = od.ProductType,
                        Price = od.Price,
                        Discount = od.Discount
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("Order not found");

            return order;
        }
        public async Task<PagedResult<OrderListDTO>> SearchOrdersAsync(OrderSearchDTO searchDto)
        {
            var query = _unitOfWork.Repository<Order>().Query().Where(o => !o.IsDeleted);

            if (!string.IsNullOrEmpty(searchDto.BuyerId))
                query = query.Where(o => o.BuyerId == searchDto.BuyerId);

            if (!string.IsNullOrEmpty(searchDto.SellerId))
                query = query.Where(o => o.SellerId == searchDto.SellerId);

            if (searchDto.Status.HasValue)
                query = query.Where(o => o.Status == searchDto.Status);

            if (searchDto.MinTotalPrice.HasValue)
                query = query.Where(o => o.TotalPrice >= searchDto.MinTotalPrice.Value);

            if (searchDto.MaxTotalPrice.HasValue)
                query = query.Where(o => o.TotalPrice <= searchDto.MaxTotalPrice.Value);

            if (searchDto.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= searchDto.FromDate.Value);

            if (searchDto.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= searchDto.ToDate.Value);

            var totalRecords = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(o => new OrderListDTO
                {
                    Id = o.Id,
                    OrderNo = o.OrderNo,
                    TotalPrice = o.TotalPrice,
                    ShippingAddress = o.ShippingAddress,
                    Discount = o.Discount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<OrderListDTO>
            {
                Items = orders,
                TotalRecords = totalRecords,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }
    }
}
