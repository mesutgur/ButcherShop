using ButcherShop.Business.Abstract;
using ButcherShop.Entity.Entities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Editor")]
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _productImageService;
        private readonly IProductService _productService;

        public ProductImageController(
            IProductImageService productImageService,
            IProductService productService)
        {
            _productImageService = productImageService;
            _productService = productService;
        }

        // GET: Admin/ProductImage/Manage/5
        public ActionResult Manage(int id)
        {
            var product = _productService.GetById(id);
            if (product == null || product.IsDeleted)
            {
                TempData["ErrorMessage"] = "Ürün bulunamadı.";
                return RedirectToAction("Index", "Product");
            }

            ViewBag.Product = product;
            var images = _productImageService.GetImagesByProductId(id);
            return View(images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int productId, string imageUrl, int displayOrder, bool? isMainImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    TempData["ErrorMessage"] = "Görsel URL'si boş olamaz.";
                    return RedirectToAction("Manage", new { id = productId });
                }

                // Checkbox işaretli değilse false kabul et
                bool isMain = isMainImage ?? false;

                // Eğer ana görsel seçiliyse, diğerlerini pasif yap
                if (isMain)
                {
                    var existingImages = _productImageService.GetImagesByProductId(productId);
                    foreach (var img in existingImages)
                    {
                        img.IsMainImage = false;
                        _productImageService.Update(img);
                    }
                }

                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImageUrl = imageUrl,
                    DisplayOrder = displayOrder,
                    IsMainImage = isMain,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };

                _productImageService.Add(productImage);

                // Eğer bu ilk görsel ve ana görsel değilse, otomatik ana yap
                var allImages = _productImageService.GetImagesByProductId(productId);
                if (allImages.Count == 1 && !isMain)
                {
                    productImage.IsMainImage = true;
                    _productImageService.Update(productImage);
                }

                TempData["SuccessMessage"] = "Görsel başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hata: " + ex.Message;
            }

            return RedirectToAction("Manage", new { id = productId });
        }

        // POST: Admin/ProductImage/SetMain
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetMain(int id)
        {
            try
            {
                _productImageService.SetMainImage(id);
                return Json(new { success = true, message = "Ana görsel ayarlandı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/ProductImage/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            try
            {
                var image = _productImageService.GetById(id);
                if (image == null)
                {
                    return Json(new { success = false, message = "Görsel bulunamadı." });
                }

                var productId = image.ProductId;
                var wasMainImage = image.IsMainImage;

                _productImageService.Delete(id);

                // Eğer silinen görsel ana görselse, ilk görseli ana yap
                if (wasMainImage)
                {
                    var remainingImages = _productImageService.GetImagesByProductId(productId);
                    if (remainingImages.Any())
                    {
                        var firstImage = remainingImages.First();
                        firstImage.IsMainImage = true;
                        _productImageService.Update(firstImage);
                    }
                }

                return Json(new { success = true, message = "Görsel başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Admin/ProductImage/UpdateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateOrder(int id, int displayOrder)
        {
            try
            {
                var image = _productImageService.GetById(id);
                if (image == null)
                {
                    return Json(new { success = false, message = "Görsel bulunamadı." });
                }

                image.DisplayOrder = displayOrder;
                image.ModifiedDate = DateTime.Now;
                _productImageService.Update(image);

                return Json(new { success = true, message = "Sıralama güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}