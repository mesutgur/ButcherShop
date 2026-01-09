using ButcherShop.DataAccess.Abstract;
using ButcherShop.DataAccess.Context;
using ButcherShop.Entity.Entities;

namespace ButcherShop.DataAccess.Concrete
{
    public class ContactMessageRepository : RepositoryBase<ContactMessage>, IContactMessageRepository
    {
        public ContactMessageRepository(ButcherShopContext context) : base(context)
        {
        }
    }
}