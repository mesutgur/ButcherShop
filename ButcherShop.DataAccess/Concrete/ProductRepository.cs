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
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ButcherShopContext context) : base(context)
        {
        }

        public List<Product> GetFeaturedProducts()
        {
            return _dbSet
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedDate)
                .ToList();
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _dbSet
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Product GetProductWithImages(int productId)
        {
            return _dbSet
                .Include(p => p.ProductImages)
                .Include(p => p.Category) // ✅ Kategori eklenmeli
                .FirstOrDefault(p => p.Id == productId && p.IsActive && !p.IsDeleted);
        }

        public List<Product> GetProductsWithCategory()
        {
            return _dbSet
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList(); // ✅ Mutlaka ToList() ile bitir
        }
    }
}
