using ButcherShop.Entity.Entities;
using System.Collections.Generic;

namespace ButcherShop.Business.Abstract
{
    public interface ICategoryService : IServiceBase<Category>
    {
        List<Category> GetActiveCategories();
        List<Category> GetCategoriesWithProducts();
        Category GetCategoryWithProducts(int categoryId);
        bool IsCategoryNameExists(string categoryName, int? excludeId = null);
    }
}