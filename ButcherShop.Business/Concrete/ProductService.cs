using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButcherShop.Business.Concrete
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) : base(productRepository)
        {
            _productRepository = productRepository;
        }

        public override void Add(Product entity)
        {
            // Business rules
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Ürün adı boş olamaz.");

            if (entity.Price <= 0)
                throw new ArgumentException("Ürün fiyatı sıfırdan büyük olmalıdır.");

            if (entity.CategoryId <= 0)
                throw new ArgumentException("Kategori seçilmelidir.");

            if (IsProductNameExists(entity.Name))
                throw new InvalidOperationException("Bu ürün adı zaten kullanılıyor.");

            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;
            entity.IsDeleted = false;

            base.Add(entity);
        }

        public override void Update(Product entity)
        {
            // Business rules
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Ürün adı boş olamaz.");

            if (entity.Price <= 0)
                throw new ArgumentException("Ürün fiyatı sıfırdan büyük olmalıdır.");

            if (IsProductNameExists(entity.Name, entity.Id))
                throw new InvalidOperationException("Bu ürün adı zaten kullanılıyor.");

            entity.ModifiedDate = DateTime.Now;

            base.Update(entity);
        }

        public override void Delete(int id)
        {
            var product = GetById(id);
            if (product == null)
                throw new ArgumentException("Ürün bulunamadı.");

            // Soft delete
            product.IsDeleted = true;
            product.IsActive = false;
            product.ModifiedDate = DateTime.Now;

            Update(product);
        }

        public List<Product> GetFeaturedProducts()
        {
            return _productRepository.GetFeaturedProducts();
        }

        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _productRepository.GetProductsByCategory(categoryId);
        }

        public Product GetProductWithImages(int productId)
        {
            return _productRepository.GetProductWithImages(productId);
        }

        public List<Product> GetProductsWithCategory()
        {
            return _productRepository.GetProductsWithCategory();
        }

        public bool IsProductNameExists(string productName, int? excludeId = null)
        {
            var query = GetAllQueryable(p => p.Name.ToLower() == productName.ToLower() && !p.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return query.Any();
        }

        public void UpdateStock(int productId, int quantity)
        {
            var product = GetById(productId);
            if (product == null)
                throw new ArgumentException("Ürün bulunamadı.");

            product.StockQuantity = quantity;
            product.ModifiedDate = DateTime.Now;

            Update(product);
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Product>();

            //return GetAll(p => p.Name.Contains(searchTerm)
            //                || p.Description.Contains(searchTerm)
            //                && p.IsActive
            //                && !p.IsDeleted);

            return GetAll(p => (p.Name.Contains(searchTerm)
                              || p.Description.Contains(searchTerm))
                              && p.IsActive
                              && !p.IsDeleted);
        }
    }
}