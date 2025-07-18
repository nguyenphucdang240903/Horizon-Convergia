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

        public async Task<List<string>> CreateOrderAsync(CreateOrderFromCartSelectionDTO dto, string buyerId)
        {
            var carts = _unitOfWork.Repository<Cart>().Query()
                .Where(c => dto.ProductIds.Contains(c.ProductId)
                    && c.BuyerId == buyerId
                    && !c.IsDeleted)
                .ToList();

            if (carts == null || !carts.Any())
                throw new Exception("No valid cart items found.");

            var productIds = carts.Select(c => c.ProductId).Distinct().ToList();
            var products = _unitOfWork.Repository<Product>()
                .Query()
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.Id))
                .ToList();

            foreach (var cart in carts)
            {
                var product = products.FirstOrDefault(p => p.Id == cart.ProductId);

                if (product == null || !product.IsVerified || product.Status != ProductStatus.Active)
                    throw new Exception($"Product {cart.ProductId} is unavailable.");

                if (cart.Quantity > product.Quantity)
                    throw new Exception($"Product {product.Model} only has {product.Quantity} item(s) left in stock.");
            }

            var groupedCarts = carts.GroupBy(c => products.First(p => p.Id == c.ProductId).SellerId);
            var orderNumbers = new List<string>();

            foreach (var sellerGroup in groupedCarts)
            {
                var orderId = Guid.NewGuid().ToString();
                var firstProduct = products.First(p => p.Id == sellerGroup.First().ProductId);

                var totalPrice = sellerGroup.Sum(c => (c.Price - c.Discount) * c.Quantity);

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
                    TotalPrice = totalPrice,
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

                var shipping = new Shipping
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = orderId,
                    ShippingFee = CalculateShippingFee(sellerGroup.Key, dto.ShippingAddress), // Hàm bạn tự định nghĩa sau
                    Status = ShippingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CarrierId = null,  // Chưa có shipper
                };

                await _unitOfWork.Repository<Shipping>().AddAsync(shipping);

                foreach (var cart in sellerGroup)
                {
                    var product = products.First(p => p.Id == cart.ProductId);
                    product.Quantity -= cart.Quantity;
                    if (product.Quantity < 0)
                        product.Quantity = 0;
                }

                orderNumbers.Add(order.OrderNo);
            }

            foreach (var cart in carts)
            {
                _unitOfWork.Repository<Cart>().Delete(cart);
            }

            await _unitOfWork.SaveAsync();
            return orderNumbers;
        }
        private decimal CalculateShippingFee(string sellerId, string buyerLocation)
        {
            decimal baseFee = 20000m;
            return baseFee;
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
        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var paymentRepo = _unitOfWork.Repository<Payment>();

            var order = orderRepo
                .Query()
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.Id == orderId && !o.IsDeleted);

            if (order == null) throw new Exception("Order not found");

            if (order.Status == newStatus) return false;

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Shipping)
            {
                bool sellerAlreadyPaid = paymentRepo.Query()
                    .Any(p =>
                        p.UserId == order.SellerId &&
                        p.Reference == $"ORDER-{order.Id}" &&
                        p.PaymentType == PaymentType.PayoutToSeller);

                if (!sellerAlreadyPaid)
                {
                    decimal platformFee = 0.1m;
                    decimal payoutAmount = order.TotalPrice * (1 - platformFee);

                    await paymentRepo.AddAsync(new Payment
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = order.SellerId,
                        Amount = payoutAmount,
                        Reference = $"ORDER-{order.Id}",
                        Description = "Payout for seller",
                        PaymentStatus = PaymentStatus.Pending,
                        PaymentType = PaymentType.PayoutToSeller,
                        PaymentMethod = "Manual",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            if (newStatus == OrderStatus.Delivered)
            {
                var shipping = _unitOfWork.Repository<Shipping>()
                    .Query()
                    .FirstOrDefault(s => s.OrderId == order.Id);

                if (shipping != null)
                {
                    bool shipperAlreadyPaid = paymentRepo.Query()
                        .Any(p =>
                            p.UserId == shipping.CarrierId &&
                            p.Reference == $"ORDER-{order.Id}" &&
                            p.PaymentType == PaymentType.PayoutToShipper);

                    if (!shipperAlreadyPaid)
                    {
                        decimal shipFee = 50000m;

                        await paymentRepo.AddAsync(new Payment
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = shipping.CarrierId,
                            Amount = shipFee,
                            Reference = $"ORDER-{order.Id}",
                            Description = "Payout for shipper",
                            PaymentStatus = PaymentStatus.Pending,
                            PaymentType = PaymentType.PayoutToShipper,
                            PaymentMethod = "Manual",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }


    }
}
