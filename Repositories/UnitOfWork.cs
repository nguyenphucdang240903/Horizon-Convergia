﻿using DataAccessObjects.Data;
using Repositories;
using Repositories.Interfaces;

namespace DataAccessObjects
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public AppDbContext Context => _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public IUserRepository Users { get; }
        public ITokenRepository Tokens { get; }

        public UnitOfWork(AppDbContext context, IUserRepository userRepository, ITokenRepository tokens)
        {
            _context = context;
            Users = userRepository;
            Tokens = tokens;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepository<T>)_repositories[typeof(T)];

            var repoInstance = new GenericRepository<T>(_context);
            _repositories.Add(typeof(T), repoInstance);
            return repoInstance;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public int Save()
        {
            return _context.SaveChanges();
        }

        public void Dispose() => _context.Dispose();
    }

}
