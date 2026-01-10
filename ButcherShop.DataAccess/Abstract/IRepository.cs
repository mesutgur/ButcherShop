using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ButcherShop.Entity.Entities;

namespace ButcherShop.DataAccess.Abstract
{
    // where T : BaseEntity: Generic tip BaseEntity'den türetilmiş olmalı (soft delete için)
    public interface IRepository<T> where T : BaseEntity
    {
        // Temel CRUD operasyonları
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);

        // Sorgulama operasyonları
        T GetById(int id);

        // Expression<Func<T, bool>>: LINQ sorguları için dinamik filtreler
        T Get(Expression<Func<T, bool>> filter);

        // IQueryable: Sorguları veritabanına göndermeden önce düzenleyebilme
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null);

        // includes: İlişkili verileri Include ile çekme (Eager Loading)
        IQueryable<T> GetAllInclude(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

        // Diğer operasyonlar
        int Count(Expression<Func<T, bool>> filter = null);
        bool Any(Expression<Func<T, bool>> filter);

        // Save
        int SaveChanges();
    }
}
