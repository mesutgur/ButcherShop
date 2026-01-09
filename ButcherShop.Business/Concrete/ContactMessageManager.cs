using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ButcherShop.Business.Concrete
{
    public class ContactMessageManager : IContactMessageService
    {
        private readonly IContactMessageRepository _repository;

        public ContactMessageManager(IContactMessageRepository repository)
        {
            _repository = repository;
        }

        public ContactMessage GetById(int id)
        {
            return _repository.GetById(id);
        }

        public List<ContactMessage> GetAll(Expression<Func<ContactMessage, bool>> filter = null)
        {
            return _repository.GetAll(filter).OrderByDescending(c => c.CreatedDate).ToList();
        }

        public void Add(ContactMessage entity)
        {
            try
            {
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.IsDeleted = false;
                entity.IsRead = false;

                // ✅ Debug için
                System.Diagnostics.Debug.WriteLine($"Adding contact message from: {entity.Name}");

                _repository.Add(entity);

                // ✅ Debug için
                System.Diagnostics.Debug.WriteLine($"Contact message added successfully");
            }
            catch (Exception ex)
            {
                // ✅ Hatayı yakala
                System.Diagnostics.Debug.WriteLine($"Error in ContactMessageManager.Add: {ex.Message}");
                throw;
            }
        }

        public void Update(ContactMessage entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _repository.Update(entity);
        }

        public void Delete(int id)
        {
            var entity = _repository.GetById(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.ModifiedDate = DateTime.Now;
                _repository.Update(entity);
            }
        }

        public void MarkAsRead(int id)
        {
            var entity = _repository.GetById(id);
            if (entity != null)
            {
                entity.IsRead = true;
                entity.ReadDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                _repository.Update(entity);
            }
        }

        public List<ContactMessage> GetUnreadMessages()
        {
            return _repository.GetAll(c => !c.IsRead && !c.IsDeleted)
                              .OrderByDescending(c => c.CreatedDate)
                              .ToList();
        }

        public int GetUnreadCount()
        {
            return _repository.GetAll(c => !c.IsRead && !c.IsDeleted).Count();
        }
    }
}