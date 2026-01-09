using ButcherShop.Entity.Entities;
using ButcherShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/User
        public async Task<ActionResult> Index()
        {
            var users = UserManager.Users.ToList();
            var userViewModels = new System.Collections.Generic.List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await UserManager.GetRolesAsync(user.Id);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/User/Create
        public ActionResult Create()
        {
            ViewBag.Roles = new MultiSelectList(
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(
                    HttpContext.GetOwinContext().Get<ButcherShop.DataAccess.Context.ButcherShopContext>()))
                    .Roles.ToList(),
                "Name",
                "Name"
            );
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Rolleri ekle
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        await UserManager.AddToRolesAsync(user.Id, model.SelectedRoles.ToArray());
                    }

                    TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            ViewBag.Roles = new MultiSelectList(
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(
                    HttpContext.GetOwinContext().Get<ButcherShop.DataAccess.Context.ButcherShopContext>()))
                    .Roles.ToList(),
                "Name",
                "Name",
                model.SelectedRoles
            );
            return View(model);
        }

        // GET: Admin/User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                IsActive = user.IsActive,
                SelectedRoles = userRoles.ToList()
            };

            ViewBag.Roles = new MultiSelectList(
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(
                    HttpContext.GetOwinContext().Get<ButcherShop.DataAccess.Context.ButcherShopContext>()))
                    .Roles.ToList(),
                "Name",
                "Name",
                userRoles
            );

            return View(model);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.City = model.City;
                user.PostalCode = model.PostalCode;
                user.IsActive = model.IsActive;

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Mevcut rolleri kaldır
                    var currentRoles = await UserManager.GetRolesAsync(user.Id);
                    await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());

                    // Yeni rolleri ekle
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        await UserManager.AddToRolesAsync(user.Id, model.SelectedRoles.ToArray());
                    }

                    TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            ViewBag.Roles = new MultiSelectList(
                new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(
                    HttpContext.GetOwinContext().Get<ButcherShop.DataAccess.Context.ButcherShopContext>()))
                    .Roles.ToList(),
                "Name",
                "Name",
                model.SelectedRoles
            );

            return View(model);
        }

        // GET: Admin/User/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            var roles = await UserManager.GetRolesAsync(user.Id);

            var model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                Roles = roles.ToList()
            };

            return View(model);
        }

        //// GET: Admin/User/Delete/5
        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return HttpNotFound();
        //    }

        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
        //        return RedirectToAction("Index");
        //    }

        //    var roles = await UserManager.GetRolesAsync(user.Id);

        //    var model = new UserViewModel
        //    {
        //        Id = user.Id,
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        UserName = user.UserName,
        //        PhoneNumber = user.PhoneNumber,
        //        Address = user.Address,
        //        City = user.City,
        //        PostalCode = user.PostalCode,
        //        IsActive = user.IsActive,
        //        CreatedDate = user.CreatedDate,
        //        Roles = roles.ToList()
        //    };

        //    ViewBag.UserRoles = roles.ToList();

        //    return View(model);
        //}

        // GET: Admin/User/Delete/5
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Kullanıcı ID'si geçersiz.";
                return RedirectToAction("Index");
            }

            var user = UserManager.FindById(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            // Kendi hesabını silmeye çalışıyor mu?
            var currentUserId = User.Identity.GetUserId();
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction("Index");
            }

            // Rolleri ViewBag'e ekle
            var roles = UserManager.GetRoles(id);
            ViewBag.UserRoles = roles;

            // Model olarak AppUser gönder
            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var currentUserId = User.Identity.GetUserId();
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Kendi hesabınızı silemezsiniz.";
                return RedirectToAction("Index");
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        // GET: Admin/User/ChangePassword/5
        public async Task<ActionResult> ChangePassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index");
            }

            ViewBag.UserName = user.UserName;

            var model = new ChangePasswordViewModel
            {
                UserId = id
            };

            return View(model);
        }


        // POST: Admin/User/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                    return RedirectToAction("Index");
                }

                // Şifreyi değiştir (admin olarak token gerektirmeden)
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, token, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi.";
                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            return View(model);
        }

        // POST: Admin/User/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ToggleStatus(string id)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                user.IsActive = !user.IsActive;
                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Json(new { success = true, isActive = user.IsActive });
                }

                return Json(new { success = false, message = "Durum güncellenemedi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}