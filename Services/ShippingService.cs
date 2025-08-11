using BusinessObjects.DTO.ShippingDTO;
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
    public class ShippingService : IShippingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object> GetUnassignedOrdersAsync(UnassignedOrdersFilterRequest filter)
        {
            var query = _unitOfWork.Repository<Order>()
                .Query()
                .Include(o => o.Shipping)
                .Include(o => o.Buyer)
                .Include(o => o.Seller)
                .Where(o => o.Shipping == null
                         || (o.Shipping.CarrierId == null && o.Shipping.Status == ShippingStatus.Pending));

            if (!string.IsNullOrEmpty(filter.BuyerName))
                query = query.Where(o => o.Buyer.Name.Contains(filter.BuyerName));

            if (!string.IsNullOrEmpty(filter.SellerName))
                query = query.Where(o => o.Seller.Name.Contains(filter.SellerName));

            if (filter.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= filter.ToDate.Value);

            var totalRecords = await query.CountAsync();
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNo,
                    o.TotalPrice,
                    o.ShippingAddress,
                    BuyerName = o.Buyer.Name,
                    SellerName = o.Seller.Name,
                    o.CreatedAt
                })
                .ToListAsync();

            return new
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Data = orders
            };
        }

        public async Task<Shipping?> AssignShipperAsync(string orderId, string carrierId)
        {
            var shipping = await _unitOfWork.Repository<Shipping>()
                .Query()
                .FirstOrDefaultAsync(s => s.OrderId == orderId);

            if (shipping == null)
            {
                shipping = new Shipping
                {
                    Id = Guid.NewGuid().ToString(),
                    CarrierId = carrierId,
                    TrackingNumber = $"TRK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}", // tránh null
                    ShippingFee = 0,
                    ActualDeliveryDate = DateTime.UtcNow,
                    Status = ShippingStatus.InTransit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderId = orderId
                };
                await _unitOfWork.Repository<Shipping>().AddAsync(shipping);
            }
            else
            {
                shipping.CarrierId = carrierId;
                shipping.Status = ShippingStatus.InTransit;
                shipping.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Shipping>().Update(shipping);
            }

            await _unitOfWork.SaveAsync();
            return shipping;
        }

    }
}
