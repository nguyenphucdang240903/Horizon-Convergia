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

        public async Task<IEnumerable<User>> SearchAsync(string keyword, int pageIndex, int pageSize)
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

            return await query
                .AsNoTracking()
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountSearchAsync(string keyword)
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

            return await query.CountAsync();
        }


    }
}
