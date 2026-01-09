using ButcherShop.Business.Abstract;
using ButcherShop.Entity.Entities;
using ButcherShop.WebUI.Helpers;
using ButcherShop.WebUI.Models;
using System.Linq;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IRecipeService _recipeService;
        private readonly IContactMessageService _contactMessageService; // ✅ EKLE

        public HomeController(
            ICategoryService categoryService,
            IProductService productService,
            IRecipeService recipeService,
            IContactMessageService contactMessageService) // ✅ EKLE
        {
            _categoryService = categoryService;
            _productService = productService;
            _recipeService = recipeService;
            _contactMessageService = contactMessageService; // ✅ EKLE
        }

        //public ActionResult Index()
        //{
        //    ViewBag.FeaturedProducts = _productService.GetFeaturedProducts().Take(6).ToList();
        //    ViewBag.Categories = _categoryService.GetActiveCategories();
        //    ViewBag.RecentRecipes = _recipeService.GetRecentRecipes(3);
        //    return View();
        //}

        public ActionResult Index()
        {
            var categories = _categoryService.GetActiveCategories();

            // ✅ DEBUG: Console'a yazdır
            foreach (var cat in categories)
            {
                System.Diagnostics.Debug.WriteLine($"Kategori: {cat.Name}, ImageUrl: {cat.ImageUrl}");
            }

            ViewBag.Categories = categories;
            ViewBag.FeaturedProducts = _productService.GetFeaturedProducts().Take(6).ToList();
            ViewBag.RecentRecipes = _recipeService.GetRecentRecipes(3);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "Hakkımızda";
            return View();
        }

        // GET: Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Title = "İletişim";
            return View();
        }

        // POST: Home/Contact - ✅ BU METODU EKLE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Lütfen tüm zorunlu alanları doğru şekilde doldurun!";
                return View(model);
            }

            try
            {
                // ✅ BURAYA BREAKPOINT KOYUN
                var contactMessage = new ContactMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Subject = model.Subject,
                    Message = model.Message
                };

                // ✅ BURAYA BREAKPOINT KOYUN - Add çalışıyor mu?
                _contactMessageService.Add(contactMessage);

                // ✅ BURAYA BREAKPOINT KOYUN - Kayıt başarılı mı?
                System.Diagnostics.Debug.WriteLine($"Message saved with ID: {contactMessage.Id}");

                // Email gönder
                bool emailSent = EmailHelper.SendContactEmail(
                    model.Name,
                    model.Email,
                    model.Phone,
                    model.Subject,
                    model.Message
                );

                if (emailSent)
                {
                    TempData["Success"] = "✅ Mesajınız başarıyla gönderildi! En kısa sürede size dönüş yapacağız.";
                }
                else
                {
                    TempData["Success"] = "✅ Mesajınız kaydedildi! En kısa sürede size dönüş yapacağız.";
                    TempData["Warning"] = "⚠️ Email bildirimi gönderilemedi, ancak mesajınız sistemimize kaydedildi.";
                }

                return RedirectToAction("Contact");
            }
            catch (System.Exception ex)
            {
                // ✅ HATA MESAJINI GÖRMEK İÇİN
                System.Diagnostics.Debug.WriteLine($"Contact form error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                TempData["Error"] = $"❌ Hata: {ex.Message}";
                return View(model);
            }
        }
    }
}