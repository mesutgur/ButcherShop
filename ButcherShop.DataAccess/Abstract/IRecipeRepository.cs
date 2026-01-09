using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.Entity.Entities;

namespace ButcherShop.DataAccess.Abstract
{
    public interface IRecipeRepository : IRepository<Recipe>
    {
        List<Recipe> GetRecentRecipes(int count);
        List<Recipe> GetPopularRecipes(int count);
        Recipe GetRecipeWithDetails(int recipeId);
        List<Recipe> GetRecipesByProduct(int productId);
        void IncrementViewCount(int recipeId);
    }
}
