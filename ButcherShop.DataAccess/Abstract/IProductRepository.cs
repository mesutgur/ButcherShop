using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.Entity.Entities;

namespace ButcherShop.DataAccess.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> GetFeaturedProducts();
        List<Product> GetProductsByCategory(int categoryId);
        Product GetProductWithImages(int productId);
        List<Product> GetProductsWithCategory();
    }
}
