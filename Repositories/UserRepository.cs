using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<IEnumerable<User>> SearchAsync(string? keyword, UserRole? role, UserStatus? status, int pageIndex, int pageSize, string sortBy, string sortOrder)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.PhoneNumber.ToLower().Contains(keyword) ||
                    u.Address.ToLower().Contains(keyword));
            }

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role);
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status);
            }

            query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
            {
                ("name", "asc") => query.OrderBy(u => u.Name),
                ("name", "desc") => query.OrderByDescending(u => u.Name),
                ("email", "asc") => query.OrderBy(u => u.Email),
                ("email", "desc") => query.OrderByDescending(u => u.Email),
                ("createdat", "asc") => query.OrderBy(u => u.CreatedAt),
                ("createdat", "desc") => query.OrderByDescending(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt) // Mặc định
            };

            return await query
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountSearchAsync(string? keyword, UserRole? role, UserStatus? status)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.Name.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.PhoneNumber.ToLower().Contains(keyword) ||
                    u.Address.ToLower().Contains(keyword));
            }

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role);
            }

            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status);
            }

            return await query.CountAsync();
        }



    }
}
