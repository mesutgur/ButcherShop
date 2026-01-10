using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "Ürün adı zorunludur")]
        [MaxLength(200, ErrorMessage = "Ürün adı en fazla 200 karakter olabilir")]
        public string Name { get; set; }

        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Birim zorunludur")]
        [MaxLength(50, ErrorMessage = "Birim en fazla 50 karakter olabilir")]
        public string Unit { get; set; } // Birim: kg, adet vb.

        [MaxLength(500, ErrorMessage = "Resim URL'i en fazla 500 karakter olabilir")]
        public string ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; } // Öne çıkan ürün mü?

        // Foreign Key
        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}
