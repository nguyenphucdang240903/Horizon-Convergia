using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> SearchAsync(string keyword, int pageIndex, int pageSize);
        Task<int> CountSearchAsync(string keyword);

    }

}
