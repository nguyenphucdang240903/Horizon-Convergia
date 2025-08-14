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
            var payments = _context.Payments
                .Where(p => p.Order != null && p.Order.SellerId == sellerId);

            if (startDate.HasValue)
                payments = payments.Where(p => p.TransactionDate >= startDate.Value);
            if (endDate.HasValue)
                payments = payments.Where(p => p.TransactionDate <= endDate.Value);

            var totalRevenue = await payments
                .Where(p => p.PaymentStatus == PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var totalProducts = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .CountAsync();

            var totalOrders = await _context.Orders
                .Where(o => o.SellerId == sellerId)
                .CountAsync();

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
    }

}
