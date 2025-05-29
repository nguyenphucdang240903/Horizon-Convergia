using System.Linq.Expressions;

namespace Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        List<T> GetAll();
        T Get(Expression<Func<T, bool>> expression);
        void Create(T entity);
        Task<T?> GetByIdAsync(object id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> Query();
    }

}
