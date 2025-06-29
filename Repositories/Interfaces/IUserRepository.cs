using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> SearchAsync(string? keyword, UserRole? role, UserStatus? status, int pageIndex, int pageSize, string sortBy, string sortOrder);
        Task<int> CountSearchAsync(string? keyword, UserRole? role, UserStatus? status);

    }

}
