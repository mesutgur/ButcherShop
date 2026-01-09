using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButcherShop.Business.Concrete
{
    public class ProductImageService : ServiceBase<ProductImage>, IProductImageService
    {
        public ProductImageService(IRepository<ProductImage> repository) : base(repository)
        {
        }

        public override void Add(ProductImage entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ImageUrl))
                throw new ArgumentException("Görsel URL'si boş olamaz.");

            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;
            entity.IsDeleted = false;

            base.Add(entity);
        }

        public List<ProductImage> GetImagesByProductId(int productId)
        {
            return GetAll(pi => pi.ProductId == productId && pi.IsActive && !pi.IsDeleted)
                .OrderBy(pi => pi.DisplayOrder)
                .ToList();
        }

        public ProductImage GetMainImage(int productId)
        {
            return Get(pi => pi.ProductId == productId && pi.IsMainImage && pi.IsActive && !pi.IsDeleted);
        }

        public void SetMainImage(int productImageId)
        {
            var newMainImage = GetById(productImageId);
            if (newMainImage == null)
                throw new ArgumentException("Görsel bulunamadı.");

            // Aynı üründeki diğer görsellerin IsMainImage'ını false yap
            var otherImages = GetAll(pi => pi.ProductId == newMainImage.ProductId && pi.Id != productImageId);
            foreach (var img in otherImages)
            {
                img.IsMainImage = false;
                img.ModifiedDate = DateTime.Now;
                Update(img);
            }

            // Yeni ana görseli ayarla
            newMainImage.IsMainImage = true;
            newMainImage.ModifiedDate = DateTime.Now;
            Update(newMainImage);
        }
    }
}