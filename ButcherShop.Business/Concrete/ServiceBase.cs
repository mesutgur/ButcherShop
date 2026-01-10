using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ButcherShop.Business.Concrete
{
    public class ServiceBase<T> : IServiceBase<T> where T : BaseEntity
    {
        protected readonly IRepository<T> _repository;

        public ServiceBase(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _repository.Add(entity); // SaveChanges is already called in repository
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _repository.Update(entity); // SaveChanges is already called in repository
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _repository.Delete(entity); // SaveChanges is already called in repository
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public virtual T GetById(int id)
        {
            return _repository.GetById(id);
        }

        public virtual T Get(Expression<Func<T, bool>> filter)
        {
            return _repository.Get(filter);
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return _repository.GetAll(filter).ToList();
        }

        public virtual IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> filter = null)
        {
            return _repository.GetAll(filter);
        }

        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            return _repository.Count(filter);
        }

        public virtual bool Any(Expression<Func<T, bool>> filter)
        {
            return _repository.Any(filter);
        }
    }
}