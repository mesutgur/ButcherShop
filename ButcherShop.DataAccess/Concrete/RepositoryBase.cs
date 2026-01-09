using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.DataAccess.Context;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ButcherShop.DataAccess.Concrete
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ButcherShopContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(ButcherShopContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Delete(entity);
            }
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            return _dbSet.FirstOrDefault(filter);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return filter == null ? _dbSet : _dbSet.Where(filter);
        }

        public IQueryable<T> GetAllInclude(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return filter == null ? query : query.Where(filter);
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            return filter == null ? _dbSet.Count() : _dbSet.Count(filter);
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Any(filter);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
