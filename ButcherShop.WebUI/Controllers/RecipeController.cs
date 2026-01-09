using ButcherShop.Business.Abstract;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IProductService _productService;

        public RecipeController(
            IRecipeService recipeService,
            IProductService productService)
        {
            _recipeService = recipeService;
            _productService = productService;
        }

        // GET: Recipe
        public ActionResult Index(string search, string difficulty, string sort)
        {
            // Tarifleri getir
            var recipes = _recipeService.GetAll(r => r.IsActive && !r.IsDeleted)
                .ToList();

            // Arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                recipes = recipes.Where(r =>
                    r.Title.ToLower().Contains(search.ToLower()) ||
                    (r.Description != null && r.Description.ToLower().Contains(search.ToLower()))
                ).ToList();
                ViewBag.SearchTerm = search;
            }

            // Zorluk filtresi
            if (!string.IsNullOrWhiteSpace(difficulty))
            {
                recipes = recipes.Where(r => r.DifficultyLevel == difficulty).ToList();
                ViewBag.SelectedDifficulty = difficulty;
            }

            // Sıralama
            switch (sort)
            {
                case "popular":
                    recipes = recipes.OrderByDescending(r => r.ViewCount).ToList();
                    break;
                case "time_asc":
                    recipes = recipes.OrderBy(r => r.PreparationTime + r.CookingTime).ToList();
                    break;
                case "time_desc":
                    recipes = recipes.OrderByDescending(r => r.PreparationTime + r.CookingTime).ToList();
                    break;
                default:
                    recipes = recipes.OrderByDescending(r => r.CreatedDate).ToList();
                    break;
            }

            ViewBag.Sort = sort;
            ViewBag.RecipeCount = recipes.Count;

            return View(recipes);
        }

        // GET: Recipe/Details/5
        public ActionResult Details(int id)
        {
            var recipe = _recipeService.GetRecipeWithDetails(id);
            if (recipe == null || recipe.IsDeleted || !recipe.IsActive)
            {
                return HttpNotFound();
            }

            // Görüntülenme sayısını artır
            _recipeService.IncrementViewCount(id);

            // Benzer tarifler (aynı zorluk seviyesinde)
            var relatedRecipes = _recipeService.GetAll(r =>
                    r.IsActive &&
                    !r.IsDeleted &&
                    r.DifficultyLevel == recipe.DifficultyLevel &&
                    r.Id != id)
                .Take(3)
                .ToList();
            ViewBag.RelatedRecipes = relatedRecipes;

            return View(recipe);
        }
    }
}