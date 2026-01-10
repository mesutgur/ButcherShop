using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Category : BaseEntity
    {
        // Kategori adı (örn: "Dana Eti", "Kuzu Eti")
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
        public string Name { get; set; }

        // Kategori açıklaması
        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
        public string Description { get; set; }

        // Kategori görseli
        [MaxLength(500, ErrorMessage = "Resim URL'i en fazla 500 karakter olabilir")]
        public string ImageUrl { get; set; }

        // Sıralama (hangi kategori önce gösterilsin)
        [Range(0, int.MaxValue, ErrorMessage = "Sıralama numarası negatif olamaz")]
        public int DisplayOrder { get; set; }

        // Navigation Property (İlişkiler)
        // Bu kategoriye ait ürünler (1-N ilişki)
        public virtual ICollection<Product> Products { get; set; }
    }
}
