using BusinessObjects.DTO.DashBoardDTO;
using BusinessObjects.Enums;
using DataAccessObjects.Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDTO> GetAdminDashboardAsync(DateTime? startDate, DateTime? endDate)
        {
            var payments = _context.Payments.AsQueryable();

            if (startDate.HasValue)
                payments = payments.Where(p => p.TransactionDate >= startDate.Value);
            if (endDate.HasValue)
                payments = payments.Where(p => p.TransactionDate <= endDate.Value);

            var totalRevenue = await payments
                .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            var transactions = await payments
                .OrderByDescending(p => p.TransactionDate)
                .Select(p => new TransactionDTO
                {
                    Id = p.Id,
                    Reference = p.Reference,
                    Amount = p.Amount,
                    TransactionDate = p.TransactionDate,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus.ToString()
                }).ToListAsync();

            return new DashboardStatsDTO
            {
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                Transactions = transactions
            };
        }

        public async Task<DashboardStatsDTO> GetSellerDashboardAsync(string sellerId, DateTime? startDate, DateTime? endDate)
        {
            // Lấy tất cả payment của seller (có hoặc không có OrderId)
            var paymentsQuery = _context.Payments
                .Include(p => p.Order)
                .Where(p =>
                    (p.Order != null && p.Order.SellerId == sellerId) // Payment gắn Order
                    || (p.UserId == sellerId)                       // Payment trực tiếp cho seller
                );

            // Lọc theo thời gian
            if (startDate.HasValue)
                paymentsQuery = paymentsQuery.Where(p => p.TransactionDate >= startDate.Value);
            if (endDate.HasValue)
                paymentsQuery = paymentsQuery.Where(p => p.TransactionDate <= endDate.Value);

            // Doanh thu (tính Completed hoặc status null)
            var totalRevenue = await paymentsQuery
                .Where(p => p.PaymentStatus == PaymentStatus.Completed || p.PaymentStatus == null)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // Tổng sản phẩm
            var productsQuery = _context.Products.Where(p => p.SellerId == sellerId);
            var totalProducts = await productsQuery.CountAsync();

            // Tổng đơn hàng
            var ordersQuery = _context.Orders.Where(o => o.SellerId == sellerId);
            if (startDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt >= startDate.Value);
            if (endDate.HasValue)
                ordersQuery = ordersQuery.Where(o => o.CreatedAt <= endDate.Value);
            var totalOrders = await ordersQuery.CountAsync();

            // Danh sách giao dịch
            var transactions = await paymentsQuery
                .OrderByDescending(p => p.TransactionDate)
                .Select(p => new TransactionDTO
                {
                    Id = p.Id,
                    Reference = p.Reference,
                    Amount = p.Amount,
                    TransactionDate = p.TransactionDate,
                    PaymentMethod = p.PaymentMethod,
                    PaymentStatus = p.PaymentStatus != null ? p.PaymentStatus.ToString() : "Unknown"
                })
                .ToListAsync();

            return new DashboardStatsDTO
            {
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalOrders = totalOrders,
                Transactions = transactions
            };
        }


    }

}
