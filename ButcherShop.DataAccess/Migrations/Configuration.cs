using ButcherShop.Entity.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace ButcherShop.DataAccess.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ButcherShop.DataAccess.Context.ButcherShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ButcherShop.DataAccess.Context.ButcherShopContext";
        }

        protected override void Seed(ButcherShop.DataAccess.Context.ButcherShopContext context)
        {
            // Roller oluşturma
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            if (!roleManager.RoleExists("Editor"))
            {
                roleManager.Create(new IdentityRole("Editor"));
            }

            if (!roleManager.RoleExists("Customer"))
            {
                roleManager.Create(new IdentityRole("Customer"));
            }

            // Admin kullanıcı oluşturma
            var userStore = new UserStore<AppUser>(context);
            var userManager = new UserManager<AppUser>(userStore);

            if (!context.Users.Any(u => u.UserName == "admin@butchershop.com"))
            {
                var adminUser = new AppUser
                {
                    UserName = "admin@butchershop.com",
                    Email = "admin@butchershop.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    PhoneNumber = "05551234567",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                userManager.Create(adminUser, "Admin123!");
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            // Örnek kategoriler
            context.Categories.AddOrUpdate(
                c => c.Name,
                new Category
                {
                    Name = "Dana Eti",
                    Description = "Taze ve kaliteli dana eti ürünleri",
                    ImageUrl = "/Content/images/categories/dana.jpg",
                    DisplayOrder = 1,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Name = "Kuzu Eti",
                    Description = "Lezzetli kuzu eti çeşitleri",
                    ImageUrl = "/Content/images/categories/kuzu.jpg",
                    DisplayOrder = 2,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Name = "Tavuk",
                    Description = "Taze tavuk ürünleri",
                    ImageUrl = "/Content/images/categories/tavuk.jpg",
                    DisplayOrder = 3,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                },
                new Category
                {
                    Name = "Şarküteri",
                    Description = "Şarküteri ürünleri ve sucuklar",
                    ImageUrl = "/Content/images/categories/sarkuteri.jpg",
                    DisplayOrder = 4,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                }
            );

            context.SaveChanges();
        }
    }
}