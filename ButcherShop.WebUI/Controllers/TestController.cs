using ButcherShop.Business.Abstract;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Controllers
{
    public class TestController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        // Constructor Injection
        public TestController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // GET: Test
        public ActionResult Index()
        {
            // Kategorileri al
            var categories = _categoryService.GetActiveCategories();
            ViewBag.CategoryCount = categories.Count;

            // Ürünleri al
            var products = _productService.GetProductsWithCategory();
            ViewBag.ProductCount = products.Count;

            return View();
        }

        // Test için örnek action
        public ActionResult Categories()
        {
            var categories = _categoryService.GetActiveCategories();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Products()
        {
            var products = _productService.GetProductsWithCategory();
            return Json(new
            {
                count = products.Count,
                products = products
            }, JsonRequestBehavior.AllowGet);
        }
    }
}