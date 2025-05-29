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

        public async Task<IEnumerable<User>> SearchAsync(string keyword) =>
            await _context.Users
                .Where(u => u.Name.Contains(keyword) || u.Email.Contains(keyword))
                .ToListAsync();
    }

}
