using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Recipe : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Ingredients { get; set; } // Malzemeler (text)
        public string Instructions { get; set; } // Yapılışı (text)
        public int PreparationTime { get; set; } // Hazırlık süresi (dakika)
        public int CookingTime { get; set; } // Pişirme süresi (dakika)
        public int ServingSize { get; set; } // Kaç kişilik
        public string DifficultyLevel { get; set; } // Zorluk: Kolay, Orta, Zor
        public int ViewCount { get; set; } // Görüntülenme sayısı

        // Foreign Key - ✅ DÜZELTME: int değil, string olmalı
        public string AuthorId { get; set; } // Identity User ID string tipindedir

        // Navigation Properties
        public virtual AppUser Author { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}

