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
            // CreatedDate, IsActive, IsDeleted are now set automatically by DbContext.SaveChanges()
            entity.IsRead = false;
            _repository.Add(entity);
        }

        public void Update(ContactMessage entity)
        {
            // ModifiedDate is now set automatically by DbContext.SaveChanges()
            _repository.Update(entity);
        }

        public void Delete(int id)
        {
            // Soft delete is now handled by RepositoryBase
            _repository.Delete(id);
        }

        public void MarkAsRead(int id)
        {
            var entity = _repository.GetById(id);
            if (entity != null)
            {
                entity.IsRead = true;
                entity.ReadDate = DateTime.Now;
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