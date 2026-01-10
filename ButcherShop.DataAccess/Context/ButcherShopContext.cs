using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButcherShop.Entity.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ButcherShop.DataAccess.Context
{
    // Identity kullanıcı yönetimi için
    public class ButcherShopContext : IdentityDbContext<AppUser>
    {
        // Connection string name defined in Web.config
        public ButcherShopContext() : base("ButcherShopConnection")
        {
            Configuration.LazyLoadingEnabled = false; // Disable lazy loading for better performance
            Configuration.ProxyCreationEnabled = false; // Disable proxy creation
        }

        // DbSet tanımlamaları (Her entity için bir tablo)
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeProduct> RecipeProducts { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // OnModelCreating : Fluent API ile ilişkileri ve kuralları tanımlama
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Identity tablolarının isimlerini özelleştirme (opsiyonel)
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");

            // Category konfigürasyonu
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Category>()
                .Property(c => c.Description)
                .HasMaxLength(500);

            // Product konfigürasyonu
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // decimal için precision

            modelBuilder.Entity<Product>()
                .Property(p => p.Unit)
                .HasMaxLength(50);

            // HasRequired/WithMany: İlişki türlerini belirleme

            // İlişkileri tanımlama
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .WillCascadeOnDelete(false); // Silme işlemlerinde cascade davranış

            modelBuilder.Entity<ProductImage>()
                .HasRequired(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .WillCascadeOnDelete(true);

            // Recipe konfigürasyonu
            modelBuilder.Entity<Recipe>()
                .Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Recipe>()
                .Property(r => r.DifficultyLevel)
                .HasMaxLength(50);

            modelBuilder.Entity<Recipe>()
                .HasRequired(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .WillCascadeOnDelete(false);

            // Many-to-Many ilişkisi (Recipe - Product)
            modelBuilder.Entity<RecipeProduct>()
                .HasRequired(rp => rp.Recipe)
                .WithMany(r => r.RecipeProducts)
                .HasForeignKey(rp => rp.RecipeId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<RecipeProduct>()
                .HasRequired(rp => rp.Product)
                .WithMany(p => p.RecipeProducts)
                .HasForeignKey(rp => rp.ProductId)
                .WillCascadeOnDelete(false);

            // Global Query Filters for Soft Delete
            // Note: EF6 doesn't support global query filters directly like EF Core
            // So we need to handle IsDeleted filtering in repository layer
            // This is already handled in GetAll methods by filtering IsDeleted = false
        }

        public static ButcherShopContext Create()
        {
            return new ButcherShopContext();
        }

        // SaveChanges override - CreatedDate ve ModifiedDate otomasyonu
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.IsActive = true;
                    entry.Entity.IsDeleted = false;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}
