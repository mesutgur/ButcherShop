using ButcherShop.Entity.Entities;
using System.Collections.Generic;

namespace ButcherShop.Business.Abstract
{
    public interface IRecipeProductService : IServiceBase<RecipeProduct>
    {
        List<RecipeProduct> GetProductsByRecipe(int recipeId);
        void AddProductsToRecipe(int recipeId, List<RecipeProduct> products);
        void UpdateRecipeProducts(int recipeId, List<RecipeProduct> products);
        void RemoveProductFromRecipe(int recipeProductId);
    }
}