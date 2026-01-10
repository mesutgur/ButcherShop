using System;
using System.Linq;
using System.Web.Mvc;
using ButcherShop.Business.Abstract;
using ButcherShop.Business.Concrete;
using ButcherShop.Entity.Entities;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IProductImageService _productImageService;
        private readonly IRecipeService _recipeService; // ✅ YENİ EKLEME

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            IProductImageService productImageService,
            IRecipeService recipeService) // ✅ YENİ EKLEME
        {
            _productService = productService;
            _categoryService = categoryService;
            _productImageService = productImageService;
            _recipeService = recipeService; // ✅ YENİ EKLEME
        }

        // GET: Admin/Product
        public ActionResult Index()
        {
            var products = _productService.GetProductsWithCategory();
            return View(products);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(
                _categoryService.GetActiveCategories(),
                "Id",
                "Name"
            );
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _productService.Add(product);
                    TempData["SuccessMessage"] = "Ürün başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            ViewBag.Categories = new SelectList(
                _categoryService.GetActiveCategories(),
                "Id",
                "Name",
                product.CategoryId
            );
            return View(product);
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int id)
        {
            var product = _productService.GetById(id);
            if (product == null || product.IsDeleted)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(
                _categoryService.GetActiveCategories(),
                "Id",
                "Name",
                product.CategoryId
            );
            return View(product);
        }

        // POST: Admin/Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(
                        _categoryService.GetActiveCategories(),
                        "Id",
                        "Name",
                        product.CategoryId
                    );
                    return View(product);
                }

                // ✅ Overposting önlemi: DB'den gerçek kaydı çek
                var existing = _productService.GetById(product.Id);
                if (existing == null || existing.IsDeleted)
                {
                    TempData["ErrorMessage"] = "Ürün bulunamadı.";
                    return RedirectToAction("Index");
                }

                // ✅ Sadece izin verilen alanları güncelle
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Unit = product.Unit;
                existing.ImageUrl = product.ImageUrl;
                existing.StockQuantity = product.StockQuantity;
                existing.IsFeatured = product.IsFeatured;
                existing.IsActive = product.IsActive;
                existing.CategoryId = product.CategoryId;

                _productService.Update(existing);

                TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            ViewBag.Categories = new SelectList(
                _categoryService.GetActiveCategories(),
                "Id",
                "Name",
                product.CategoryId
            );
            return View(product);
        }


        // GET: Admin/Product/Details/5
        public ActionResult Details(int id)
        {
            var product = _productService.GetProductWithImages(id);
            if (product == null || product.IsDeleted)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index");
            }

            // Bu ürünü kullanan tarifleri getir
            var relatedRecipes = _recipeService.GetRecipesByProduct(id);
            ViewBag.RelatedRecipes = relatedRecipes;

            return View(product);
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int id)
        {
            var product = _productService.GetById(id);
            if (product == null || product.IsDeleted)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _productService.Delete(id);
                TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Product/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                var product = _productService.GetById(id);
                if (product != null && !product.IsDeleted)
                {
                    product.IsActive = !product.IsActive;
                    _productService.Update(product);
                    return Json(new { success = true, isActive = product.IsActive });
                }
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/Product/ToggleFeatured/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleFeatured(int id)
        {
            try
            {
                var product = _productService.GetById(id);
                if (product != null && !product.IsDeleted)
                {
                    product.IsFeatured = !product.IsFeatured;
                    _productService.Update(product);
                    return Json(new { success = true, isFeatured = product.IsFeatured });
                }
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/Product/UpdateStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateStock(int id, int quantity)
        {
            try
            {
                _productService.UpdateStock(id, quantity);
                return Json(new { success = true, message = "Stok güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}