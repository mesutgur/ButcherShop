using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButcherShop.Business.Concrete
{
    public class RecipeProductService : ServiceBase<RecipeProduct>, IRecipeProductService
    {
        public RecipeProductService(IRepository<RecipeProduct> repository) : base(repository)
        {
        }

        public List<RecipeProduct> GetProductsByRecipe(int recipeId)
        {
            return GetAll(rp => rp.RecipeId == recipeId && rp.IsActive && !rp.IsDeleted);
        }

        public void AddProductsToRecipe(int recipeId, List<RecipeProduct> products)
        {
            if (products == null || !products.Any())
                throw new ArgumentException("Ürün listesi boş olamaz.");

            foreach (var product in products)
            {
                product.RecipeId = recipeId;
                product.CreatedDate = DateTime.Now;
                product.IsActive = true;
                product.IsDeleted = false;
                Add(product);
            }
        }

        public void UpdateRecipeProducts(int recipeId, List<RecipeProduct> products)
        {
            // Önce mevcut ürünleri sil
            var existingProducts = GetProductsByRecipe(recipeId);
            foreach (var existing in existingProducts)
            {
                Delete(existing);
            }

            // Yeni ürünleri ekle
            AddProductsToRecipe(recipeId, products);
        }

        public void RemoveProductFromRecipe(int recipeProductId)
        {
            Delete(recipeProductId);
        }
    }
}