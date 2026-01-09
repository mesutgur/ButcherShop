using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.DataAccess.Context;
using ButcherShop.Entity.Entities;
using System.Data.Entity;


namespace ButcherShop.DataAccess.Concrete
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ButcherShopContext context) : base(context)
        {
        }

        public List<Category> GetActiveCategories()
        {
            return _dbSet
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
        }

        public List<Category> GetCategoriesWithProducts()
        {
            return _dbSet
                .Include(c => c.Products)
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
        }

        public Category GetCategoryWithProducts(int categoryId)
        {
            return _dbSet
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == categoryId && c.IsActive && !c.IsDeleted);
        }
    }
}
