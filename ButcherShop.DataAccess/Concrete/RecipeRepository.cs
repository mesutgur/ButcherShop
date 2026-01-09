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
    public class RecipeRepository : RepositoryBase<Recipe>, IRecipeRepository
    {
        public RecipeRepository(ButcherShopContext context) : base(context)
        {
        }

        public List<Recipe> GetRecentRecipes(int count)
        {
            return _dbSet
                .Include(r => r.Author)
                .Where(r => r.IsActive && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .Take(count)
                .ToList();
        }

        public List<Recipe> GetPopularRecipes(int count)
        {
            return _dbSet
                .Include(r => r.Author)
                .Where(r => r.IsActive && !r.IsDeleted)
                .OrderByDescending(r => r.ViewCount)
                .Take(count)
                .ToList();
        }

        public Recipe GetRecipeWithDetails(int recipeId)
        {
            return _dbSet
                .Include(r => r.Author)
                .Include(r => r.RecipeProducts.Select(rp => rp.Product))
                .FirstOrDefault(r => r.Id == recipeId && r.IsActive && !r.IsDeleted);
        }

        public List<Recipe> GetRecipesByProduct(int productId)
        {
            return _dbSet
                .Include(r => r.Author)
                .Where(r => r.RecipeProducts.Any(rp => rp.ProductId == productId)
                         && r.IsActive && !r.IsDeleted)
                .OrderByDescending(r => r.ViewCount)
                .ToList();
        }

        public void IncrementViewCount(int recipeId)
        {
            var recipe = GetById(recipeId);
            if (recipe != null)
            {
                recipe.ViewCount++;
                SaveChanges();
            }
        }
    }
}
