using ButcherShop.Business.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Admin/Category
        // Index: Tüm kategorileri listeler
        public ActionResult Index()
        {
            var categories = _categoryService.GetAll(c => !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToList();
            return View(categories);
        }

        // GET: Admin/Category/Create
        // Create (GET/POST): Yeni kategori ekler
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        // Create (GET/POST): Yeni kategori ekler
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _categoryService.Add(category);
                    TempData["SuccessMessage"] = "Kategori başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return View(category);
        }

        // GET: Admin/Category/Edit/5
        // Edit (GET/POST): Kategori düzenler
        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null || category.IsDeleted)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // Edit (GET/POST): Kategori düzenler
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Category category)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _categoryService.Update(category);
        //            TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Hata: " + ex.Message;
        //    }

        //    return View(category);
        //}

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                // ✅ Overposting önlemi
                var existing = _categoryService.GetById(category.Id);
                if (existing == null || existing.IsDeleted)
                {
                    TempData["ErrorMessage"] = "Kategori bulunamadı.";
                    return RedirectToAction("Index");
                }

                // ✅ Sadece izin verilen alanlar
                existing.Name = category.Name;
                existing.Description = category.Description;
                existing.ImageUrl = category.ImageUrl;
                existing.DisplayOrder = category.DisplayOrder;
                existing.IsActive = category.IsActive;

                _categoryService.Update(existing);

                TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
                return View(category);
            }
        }


        // GET: Admin/Category/Delete/5
        // Delete (GET/POST): Kategori siler (Soft Delete)
        public ActionResult Delete(int id)
        {
            var category = _categoryService.GetById(id);
            if (category == null || category.IsDeleted)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        // Delete (GET/POST): Kategori siler (Soft Delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _categoryService.Delete(id);
                TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // POST: Admin/Category/ToggleStatus/5
        // ToggleStatus (AJAX): Aktif/Pasif durumunu değiştirir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleStatus(int id)
        {
            try
            {
                var category = _categoryService.GetById(id);
                if (category != null && !category.IsDeleted)
                {
                    category.IsActive = !category.IsActive;
                    _categoryService.Update(category);
                    return Json(new { success = true, isActive = category.IsActive });
                }
                return Json(new { success = false, message = "Kategori bulunamadı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}