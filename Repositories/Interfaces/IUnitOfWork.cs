using Repositories.Interfaces;


namespace DataAccessObjects
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        IUserRepository Users { get; }
        ITokenRepository Tokens { get; }
        int Save();
        Task<int> SaveAsync();
    }

}
