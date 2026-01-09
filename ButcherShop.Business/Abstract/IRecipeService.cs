using ButcherShop.Entity.Entities;
using System.Collections.Generic;

namespace ButcherShop.Business.Abstract
{
    public interface IRecipeService : IServiceBase<Recipe>
    {
        List<Recipe> GetRecentRecipes(int count);
        List<Recipe> GetPopularRecipes(int count);
        Recipe GetRecipeWithDetails(int recipeId);
        List<Recipe> GetRecipesByProduct(int productId);
        List<Recipe> GetRecipesByAuthor(string authorId);
        void IncrementViewCount(int recipeId);
        List<Recipe> SearchRecipes(string searchTerm);
    }
}