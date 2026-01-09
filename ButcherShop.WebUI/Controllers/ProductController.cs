using ButcherShop.Business.Abstract;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IRecipeService _recipeService;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IRecipeService recipeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _recipeService = recipeService;
        }

        // GET: Product
        public ActionResult Index(int? categoryId, string search, string sort)
        {
            // Kategoriler (Sidebar için)
            ViewBag.Categories = _categoryService.GetActiveCategories();
            ViewBag.SelectedCategory = categoryId;

            // Ürünleri getir - Category ve ProductImages ile birlikte
            var products = _productService.GetProductsWithCategory()
                .Where(p => p.IsActive && !p.IsDeleted)
                .ToList();

            // Kategori filtresi
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            // Arama
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.Name.ToLower().Contains(search.ToLower()) ||
                    (p.Description != null && p.Description.ToLower().Contains(search.ToLower()))
                ).ToList();
                ViewBag.SearchTerm = search;
            }

            // Sıralama
            switch (sort)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price).ToList();
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price).ToList();
                    break;
                case "name":
                    products = products.OrderBy(p => p.Name).ToList();
                    break;
                default:
                    products = products.OrderByDescending(p => p.CreatedDate).ToList();
                    break;
            }

            ViewBag.Sort = sort;
            ViewBag.ProductCount = products.Count;

            return View(products);
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            // Ürünü görselleri ve kategorisi ile birlikte getir
            var product = _productService.GetProductWithImages(id);
            if (product == null || product.IsDeleted || !product.IsActive)
            {
                return HttpNotFound();
            }

            // İlgili tarifler
            var relatedRecipes = _recipeService.GetRecipesByProduct(id);
            ViewBag.RelatedRecipes = relatedRecipes;

            // Aynı kategorideki diğer ürünler
            var relatedProducts = _productService.GetProductsByCategory(product.CategoryId)
                .Where(p => p.Id != id)
                .Take(4)
                .ToList();
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}