using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.Entity.Entities;

namespace ButcherShop.DataAccess.Abstract
{
    public interface ICategoryRepository : IRepository<Category>
    {
        List<Category> GetActiveCategories();
        List<Category> GetCategoriesWithProducts();
        Category GetCategoryWithProducts(int categoryId);
    }
}
