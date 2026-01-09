using ButcherShop.Entity.Entities;
using System.Collections.Generic;

namespace ButcherShop.Business.Abstract
{
    public interface IProductImageService : IServiceBase<ProductImage>
    {
        List<ProductImage> GetImagesByProductId(int productId);
        ProductImage GetMainImage(int productId);
        void SetMainImage(int productImageId);
    }
}