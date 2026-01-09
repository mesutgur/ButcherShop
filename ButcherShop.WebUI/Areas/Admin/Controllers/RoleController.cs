using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ButcherShop.WebUI.App_Start;
using ButcherShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace ButcherShop.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<Entity.Entities.AppUser> _userManager;

        // HttpContext ctor’da null gelebileceği için burada lazy init yapıyoruz.
        private DataAccess.Context.ButcherShopContext DbContext
            => HttpContext.GetOwinContext().Get<DataAccess.Context.ButcherShopContext>();

        private RoleManager<IdentityRole> RoleManager
        {
            get
            {
                if (_roleManager != null) return _roleManager;
                _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(DbContext));
                return _roleManager;
            }
        }

        private UserManager<Entity.Entities.AppUser> UserManager
        {
            get
            {
                if (_userManager != null) return _userManager;
                _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
        }

        // GET: Admin/Role
        public ActionResult Index()
        {
            var roles = RoleManager.Roles.ToList();

            var roleViewModels = roles
                .Select(role => new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    // N+1 yok: kullanıcıların RoleId listesi üzerinden say
                    UserCount = UserManager.Users.Count(u => u.Roles.Any(ur => ur.RoleId == role.Id))
                })
                .OrderBy(r => r.Name)
                .ToList();

            return View(roleViewModels);
        }

        // GET: Admin/Role/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await RoleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Bu rol adı zaten kullanılıyor.");
                return View(model);
            }

            var role = new IdentityRole(model.Name);
            var result = await RoleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Rol başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        // GET: Admin/Role/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            var usersInRole = UserManager.Users
                .Where(u => u.Roles.Any(ur => ur.RoleId == role.Id))
                .Select(u => u.UserName)
                .ToList();

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                Users = usersInRole
            };

            return View(model);
        }

        // POST: Admin/Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditRoleViewModel model)
        {
            // ModelState hatalıysa: kullanıcı listesini role.Id üzerinden tekrar doldur (model.Name ile değil!)
            if (!ModelState.IsValid)
            {
                var roleForList = await RoleManager.FindByIdAsync(model.Id);
                model.Users = roleForList == null
                    ? new List<string>()
                    : UserManager.Users
                        .Where(u => u.Roles.Any(ur => ur.RoleId == roleForList.Id))
                        .Select(u => u.UserName)
                        .ToList();

                return View(model);
            }

            var role = await RoleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            // Rol adı değiştiyse: aynı isim var mı?
            if (!string.Equals(role.Name, model.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await RoleManager.RoleExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name", "Bu rol adı zaten kullanılıyor.");

                    // listeyi role.Id üzerinden yeniden yükle
                    model.Users = UserManager.Users
                        .Where(u => u.Roles.Any(ur => ur.RoleId == role.Id))
                        .Select(u => u.UserName)
                        .ToList();

                    return View(model);
                }

                role.Name = model.Name;
                var result = await RoleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error);

                    // listeyi role.Id üzerinden yeniden yükle
                    model.Users = UserManager.Users
                        .Where(u => u.Roles.Any(ur => ur.RoleId == role.Id))
                        .Select(u => u.UserName)
                        .ToList();

                    return View(model);
                }
            }

            TempData["SuccessMessage"] = "Rol başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // GET: Admin/Role/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            var usersInRole = UserManager.Users
                .Where(u => u.Roles.Any(ur => ur.RoleId == role.Id))
                .ToList();

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                UserCount = usersInRole.Count
            };

            ViewBag.Users = usersInRole;
            return View(model);
        }

        // GET: Admin/Role/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            var userCount = UserManager.Users.Count(u => u.Roles.Any(ur => ur.RoleId == role.Id));

            var model = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name,
                UserCount = userCount
            };

            return View(model);
        }

        // POST: Admin/Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            // Sistem rolleri korunmalı
            var systemRoles = new[] { "Admin", "Editor", "Customer" };
            if (systemRoles.Contains(role.Name))
            {
                TempData["ErrorMessage"] = "Sistem rolleri silinemez.";
                return RedirectToAction("Index");
            }

            // Rolde kullanıcı varsa silme
            var userCount = UserManager.Users.Count(u => u.Roles.Any(ur => ur.RoleId == role.Id));
            if (userCount > 0)
            {
                TempData["ErrorMessage"] = "Bu rolde kullanıcılar bulunduğu için silinemez. Önce kullanıcıları başka rollere taşıyın.";
                return RedirectToAction("Index");
            }

            var result = await RoleManager.DeleteAsync(role);
            if (result.Succeeded)
                TempData["SuccessMessage"] = "Rol başarıyla silindi.";
            else
                TempData["ErrorMessage"] = "Rol silinirken bir hata oluştu.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _roleManager?.Dispose();
                _roleManager = null;
                _userManager = null; // OWIN yönetir, dispose etmiyoruz
            }

            base.Dispose(disposing);
        }
    }
}
