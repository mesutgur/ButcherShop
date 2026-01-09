using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; } // Birim: kg, adet vb.
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFeatured { get; set; } // Öne çıkan ürün mü?

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}
