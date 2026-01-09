using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ButcherShop.Business.Abstract
{
    public interface IContactMessageService
    {
        ContactMessage GetById(int id);
        List<ContactMessage> GetAll(Expression<Func<ContactMessage, bool>> filter = null);
        void Add(ContactMessage entity);
        void Update(ContactMessage entity);
        void Delete(int id);
        void MarkAsRead(int id);
        List<ContactMessage> GetUnreadMessages();
        int GetUnreadCount();
    }
}