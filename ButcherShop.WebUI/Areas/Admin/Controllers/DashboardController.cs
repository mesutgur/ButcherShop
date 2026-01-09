using System.Linq;
using System.Web.Mvc;
using ButcherShop.Business.Abstract;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IRecipeService _recipeService;

        public DashboardController(
            ICategoryService categoryService,
            IProductService productService,
            IRecipeService recipeService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _recipeService = recipeService;
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            ViewBag.CategoryCount = _categoryService.Count(c => c.IsActive && !c.IsDeleted);
            ViewBag.ProductCount = _productService.Count(p => p.IsActive && !p.IsDeleted);
            ViewBag.RecipeCount = _recipeService.Count(r => r.IsActive && !r.IsDeleted);

            // Son eklenen ürünler
            var recentProducts = _productService.GetAll(p => p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToList();

            return View(recentProducts);
        }
    }
}