using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ButcherShop.Entity.Entities;

namespace ButcherShop.Business.Abstract
{
    public interface IServiceBase<T> where T : BaseEntity
    {
        // Temel CRUD operasyonları
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);

        // Sorgulama operasyonları
        T GetById(int id);
        T Get(Expression<Func<T, bool>> filter);
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        IQueryable<T> GetAllQueryable(Expression<Func<T, bool>> filter = null);

        // Diğer operasyonlar
        int Count(Expression<Func<T, bool>> filter = null);
        bool Any(Expression<Func<T, bool>> filter);
    }
}