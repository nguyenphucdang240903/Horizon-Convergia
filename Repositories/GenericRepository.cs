using DataAccessObjects.Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public virtual List<T> GetAll()
        {
            return _context.Set<T>().ToList();
            //return _context.Set<T>().AsNoTracking().ToList();
        }
        public virtual T Get(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }
        public void Create(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }
        public virtual async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual void Update(T entity) => _dbSet.Update(entity);
        public virtual void Delete(T entity) => _dbSet.Remove(entity);
        public IQueryable<T> Query() => _dbSet.AsQueryable();
    }

}
