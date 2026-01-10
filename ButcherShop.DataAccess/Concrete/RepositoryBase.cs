using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.DataAccess.Context;
using ButcherShop.Entity.Entities;
using System.Data.Entity;
using System.Linq.Expressions;

namespace ButcherShop.DataAccess.Concrete
{
    public class RepositoryBase<T> : IRepository<T> where T : BaseEntity
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
            // Soft delete implementation
            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            Update(entity);
        }

        public void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Delete(entity); // SaveChanges already called in Delete(entity)
            }
        }

        public T GetById(int id)
        {
            var entity = _dbSet.Find(id);
            return entity != null && !entity.IsDeleted ? entity : null;
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(e => !e.IsDeleted).FirstOrDefault(filter);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet.Where(e => !e.IsDeleted); // Soft delete filter
            return filter == null ? query : query.Where(filter);
        }

        public IQueryable<T> GetAllInclude(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(e => !e.IsDeleted); // Soft delete filter

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return filter == null ? query : query.Where(filter);
        }

        public int Count(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet.Where(e => !e.IsDeleted);
            return filter == null ? query.Count() : query.Count(filter);
        }

        public bool Any(Expression<Func<T, bool>> filter)
        {
            return _dbSet.Where(e => !e.IsDeleted).Any(filter);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
