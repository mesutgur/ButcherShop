using ButcherShop.Business.Abstract;
using ButcherShop.DataAccess.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ButcherShop.Business.Concrete
{
    public class RecipeService : ServiceBase<Recipe>, IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository) : base(recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public override void Add(Recipe entity)
        {
            // Business rules
            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Tarif başlığı boş olamaz.");

            if (string.IsNullOrWhiteSpace(entity.Instructions))
                throw new ArgumentException("Tarif yapılışı boş olamaz.");

            if (string.IsNullOrWhiteSpace(entity.AuthorId))
                throw new ArgumentException("Yazar bilgisi eksik.");

            entity.CreatedDate = DateTime.Now;
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.ViewCount = 0;

            base.Add(entity);
        }

        public override void Update(Recipe entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Title))
                throw new ArgumentException("Tarif başlığı boş olamaz.");

            entity.ModifiedDate = DateTime.Now;

            base.Update(entity);
        }

        public override void Delete(int id)
        {
            var recipe = GetById(id);
            if (recipe == null)
                throw new ArgumentException("Tarif bulunamadı.");

            // Soft delete
            recipe.IsDeleted = true;
            recipe.IsActive = false;
            recipe.ModifiedDate = DateTime.Now;

            Update(recipe);
        }

        public List<Recipe> GetRecentRecipes(int count)
        {
            return _recipeRepository.GetRecentRecipes(count);
        }

        public List<Recipe> GetPopularRecipes(int count)
        {
            return _recipeRepository.GetPopularRecipes(count);
        }

        public Recipe GetRecipeWithDetails(int recipeId)
        {
            return _recipeRepository.GetRecipeWithDetails(recipeId);
        }

        public List<Recipe> GetRecipesByProduct(int productId)
        {
            return _recipeRepository.GetRecipesByProduct(productId);
        }

        public List<Recipe> GetRecipesByAuthor(string authorId)
        {
            return GetAll(r => r.AuthorId == authorId && r.IsActive && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public void IncrementViewCount(int recipeId)
        {
            _recipeRepository.IncrementViewCount(recipeId);
        }

        public List<Recipe> SearchRecipes(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Recipe>();

            return GetAll(r => (r.Title.Contains(searchTerm)
                             || r.Description.Contains(searchTerm)
                             || r.Ingredients.Contains(searchTerm))
                            && r.IsActive
                            && !r.IsDeleted);
        }
    }
}