using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButcherShop.Business.Concrete
{

    // Kategori adı boş olamaz
    // Aynı isimde iki kategori olamaz
    // Silme işlemi soft delete(IsDeleted = true)
    // CreatedDate ve ModifiedDate otomatik set edilir
    public class CategoryService : ServiceBase<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override void Add(Category entity)
        {
            // Business rules
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Kategori adı boş olamaz.");

            if (IsCategoryNameExists(entity.Name))
                throw new InvalidOperationException("Bu kategori adı zaten kullanılıyor.");

            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;
            entity.IsDeleted = false;

            base.Add(entity);
        }

        public override void Update(Category entity)
        {
            // Business rules
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentException("Kategori adı boş olamaz.");

            if (IsCategoryNameExists(entity.Name, entity.Id))
                throw new InvalidOperationException("Bu kategori adı zaten kullanılıyor.");

            entity.ModifiedDate = DateTime.Now;

            base.Update(entity);
        }

        public override void Delete(int id)
        {
            var category = GetById(id);
            if (category == null)
                throw new ArgumentException("Kategori bulunamadı.");

            // Soft delete
            category.IsDeleted = true;
            category.IsActive = false;
            category.ModifiedDate = DateTime.Now;

            Update(category);
        }

        public List<Category> GetActiveCategories()
        {
            return _categoryRepository.GetActiveCategories();
        }

        public List<Category> GetCategoriesWithProducts()
        {
            return _categoryRepository.GetCategoriesWithProducts();
        }

        public Category GetCategoryWithProducts(int categoryId)
        {
            return _categoryRepository.GetCategoryWithProducts(categoryId);
        }

        public bool IsCategoryNameExists(string categoryName, int? excludeId = null)
        {
            var query = GetAllQueryable(c => c.Name.ToLower() == categoryName.ToLower() && !c.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return query.Any();
        }
    }
}