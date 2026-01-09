using ButcherShop.Business.Abstract;
using ButcherShop.Entity.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IProductService _productService;
        private readonly IRecipeProductService _recipeProductService;

        public RecipeController(
            IRecipeService recipeService,
            IProductService productService,
            IRecipeProductService recipeProductService)
        {
            _recipeService = recipeService;
            _productService = productService;
            _recipeProductService = recipeProductService;
        }

        /*
         * AuthorId: Giriş yapan kullanıcının ID'si otomatik atanır
         * selectedProducts & quantities: Tarifin kullandığı ürünler ve miktarları
         * [ValidateInput(false)]: HTML içeriğe izin verir (Ingredients, Instructions)
         * RecipeProduct ilişkisi: Many-to-Many ilişki yönetimi
         */

        // GET: Admin/Recipe
        public ActionResult Index()
        {
            var recipes = _recipeService.GetAll(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
            return View(recipes);
        }

        // GET: Admin/Recipe/Create
        public ActionResult Create()
        {
            ViewBag.Products = new MultiSelectList(
                _productService.GetAll(p => p.IsActive && !p.IsDeleted),
                "Id",
                "Name"
            );
            return View();
        }

        // POST: Admin/Recipe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)] // HTML içerik için
        public ActionResult Create(Recipe recipe, int[] selectedProducts, string[] quantities)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Giriş yapan kullanıcının ID'sini al
                    recipe.AuthorId = User.Identity.GetUserId();

                    _recipeService.Add(recipe);

                    // Ürünleri ekle
                    if (selectedProducts != null && selectedProducts.Length > 0)
                    {
                        for (int i = 0; i < selectedProducts.Length; i++)
                        {
                            var recipeProduct = new RecipeProduct
                            {
                                RecipeId = recipe.Id,
                                ProductId = selectedProducts[i],
                                Quantity = quantities != null && i < quantities.Length ? quantities[i] : "",
                                CreatedDate = DateTime.Now,
                                IsActive = true,
                                IsDeleted = false
                            };
                            _recipeProductService.Add(recipeProduct);
                        }
                    }

                    TempData["SuccessMessage"] = "Tarif başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            ViewBag.Products = new MultiSelectList(
                _productService.GetAll(p => p.IsActive && !p.IsDeleted),
                "Id",
                "Name",
                selectedProducts
            );
            return View(recipe);
        }

        // GET: Admin/Recipe/Edit/5
        public ActionResult Edit(int id)
        {
            var recipe = _recipeService.GetRecipeWithDetails(id);
            if (recipe == null || recipe.IsDeleted)
            {
                TempData["ErrorMessage"] = "Tarif bulunamadı.";
                return RedirectToAction("Index");
            }

            ViewBag.Products = new MultiSelectList(
                _productService.GetAll(p => p.IsActive && !p.IsDeleted),
                "Id",
                "Name",
                recipe.RecipeProducts?.Select(rp => rp.ProductId)
            );

            return View(recipe);
        }

        // POST: Admin/Recipe/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Edit(Recipe recipe, int[] selectedProducts, string[] quantities)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _recipeService.Update(recipe);

        //            // Mevcut ürünleri sil ve yenilerini ekle
        //            var existingProducts = _recipeProductService.GetProductsByRecipe(recipe.Id);
        //            foreach (var rp in existingProducts)
        //            {
        //                _recipeProductService.Delete(rp.Id);
        //            }

        //            // Yeni ürünleri ekle
        //            if (selectedProducts != null && selectedProducts.Length > 0)
        //            {
        //                for (int i = 0; i < selectedProducts.Length; i++)
        //                {
        //                    var recipeProduct = new RecipeProduct
        //                    {
        //                        RecipeId = recipe.Id,
        //                        ProductId = selectedProducts[i],
        //                        Quantity = quantities != null && i < quantities.Length ? quantities[i] : "",
        //                        CreatedDate = DateTime.Now,
        //                        IsActive = true,
        //                        IsDeleted = false
        //                    };
        //                    _recipeProductService.Add(recipeProduct);
        //                }
        //            }

        //            TempData["SuccessMessage"] = "Tarif başarıyla güncellendi.";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Hata: " + ex.Message;
        //    }

        //    ViewBag.Products = new MultiSelectList(
        //        _productService.GetAll(p => p.IsActive && !p.IsDeleted),
        //        "Id",
        //        "Name",
        //        selectedProducts
        //    );
        //    return View(recipe);
        //}

        // POST: Admin/Recipe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Recipe recipe, int[] selectedProducts, string[] quantities)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Products = new MultiSelectList(
                        _productService.GetAll(p => p.IsActive && !p.IsDeleted),
                        "Id",
                        "Name",
                        selectedProducts
                    );
                    return View(recipe);
                }

                // ✅ Overposting önlemi: DB'den mevcut tarifi çek
                var existing = _recipeService.GetRecipeWithDetails(recipe.Id);
                if (existing == null || existing.IsDeleted)
                {
                    TempData["ErrorMessage"] = "Tarif bulunamadı.";
                    return RedirectToAction("Index");
                }

                // ✅ Sadece izin verilen alanlar
                existing.Title = recipe.Title;
                existing.Description = recipe.Description;
                existing.ImageUrl = recipe.ImageUrl;
                existing.Ingredients = recipe.Ingredients;
                existing.Instructions = recipe.Instructions;
                existing.PreparationTime = recipe.PreparationTime;
                existing.CookingTime = recipe.CookingTime;
                existing.ServingSize = recipe.ServingSize;
                existing.DifficultyLevel = recipe.DifficultyLevel;
                existing.IsActive = recipe.IsActive;

                // ❗ Korunan alanlar: AuthorId, ViewCount, CreatedDate, IsDeleted vs.
                _recipeService.Update(existing);

                // Ürün ilişkileri: mevcutları kaldır (şu an fiziksel delete çalışıyor)
                var existingProducts = _recipeProductService.GetProductsByRecipe(existing.Id);
                foreach (var rp in existingProducts)
                {
                    _recipeProductService.Delete(rp.Id);
                }

                // Yenilerini ekle
                if (selectedProducts != null && selectedProducts.Length > 0)
                {
                    for (int i = 0; i < selectedProducts.Length; i++)
                    {
                        var recipeProduct = new RecipeProduct
                        {
                            RecipeId = existing.Id,
                            ProductId = selectedProducts[i],
                            Quantity = quantities != null && i < quantities.Length ? quantities[i] : "",
                            CreatedDate = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                        _recipeProductService.Add(recipeProduct);
                    }
                }

                TempData["SuccessMessage"] = "Tarif başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            ViewBag.Products = new MultiSelectList(
                _productService.GetAll(p => p.IsActive && !p.IsDeleted),
                "Id",
                "Name",
                selectedProducts
            );
            return View(recipe);
        }


        // GET: Admin/Recipe/Details/5
        public ActionResult Details(int id)
        {
            var recipe = _recipeService.GetRecipeWithDetails(id);
            if (recipe == null || recipe.IsDeleted)
            {
                TempData["ErrorMessage"] = "Tarif bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        // GET: Admin/Recipe/Delete/5
        public ActionResult Delete(int id)
        {
            var recipe = _recipeService.GetById(id);
            if (recipe == null || recipe.IsDeleted)
            {
                TempData["ErrorMessage"] = "Tarif bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        // POST: Admin/Recipe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _recipeService.Delete(id);
                TempData["SuccessMessage"] = "Tarif başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Recipe/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                var recipe = _recipeService.GetById(id);
                if (recipe != null && !recipe.IsDeleted)
                {
                    recipe.IsActive = !recipe.IsActive;
                    _recipeService.Update(recipe);
                    return Json(new { success = true, isActive = recipe.IsActive });
                }
                return Json(new { success = false, message = "Tarif bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}