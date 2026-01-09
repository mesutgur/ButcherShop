using ButcherShop.Entity.Entities;
using System.Collections.Generic;

namespace ButcherShop.Business.Abstract
{
    public interface IProductService : IServiceBase<Product>
    {
        List<Product> GetFeaturedProducts();
        List<Product> GetProductsByCategory(int categoryId);
        Product GetProductWithImages(int productId);
        List<Product> GetProductsWithCategory();
        bool IsProductNameExists(string productName, int? excludeId = null);
        void UpdateStock(int productId, int quantity);
        List<Product> SearchProducts(string searchTerm);
    }
}