using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Category : BaseEntity
    {
        // Kategori adı (örn: "Dana Eti", "Kuzu Eti")
        public string Name { get; set; }

        // Kategori açıklaması
        public string Description { get; set; }

        // Kategori görseli
        public string ImageUrl { get; set; }

        // Sıralama (hangi kategori önce gösterilsin)
        public int DisplayOrder { get; set; }

        // Navigation Property (İlişkiler)
        // Bu kategoriye ait ürünler (1-N ilişki)
        public virtual ICollection<Product> Products { get; set; }
    }
}
