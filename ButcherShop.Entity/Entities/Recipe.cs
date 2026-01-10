using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class Recipe : BaseEntity
    {
        [Required(ErrorMessage = "Tarif başlığı zorunludur")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = "Resim URL'i en fazla 500 karakter olabilir")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Malzemeler zorunludur")]
        public string Ingredients { get; set; } // Malzemeler (text)

        [Required(ErrorMessage = "Yapılışı zorunludur")]
        public string Instructions { get; set; } // Yapılışı (text)

        [Range(0, int.MaxValue, ErrorMessage = "Hazırlık süresi negatif olamaz")]
        public int PreparationTime { get; set; } // Hazırlık süresi (dakika)

        [Range(0, int.MaxValue, ErrorMessage = "Pişirme süresi negatif olamaz")]
        public int CookingTime { get; set; } // Pişirme süresi (dakika)

        [Range(1, 100, ErrorMessage = "Porsiyon sayısı 1-100 arasında olmalıdır")]
        public int ServingSize { get; set; } // Kaç kişilik

        [MaxLength(50, ErrorMessage = "Zorluk seviyesi en fazla 50 karakter olabilir")]
        public string DifficultyLevel { get; set; } // Zorluk: Kolay, Orta, Zor

        public int ViewCount { get; set; } // Görüntülenme sayısı

        // Foreign Key - Identity User ID string tipindedir
        [Required(ErrorMessage = "Yazar bilgisi zorunludur")]
        public string AuthorId { get; set; }

        // Navigation Properties
        public virtual AppUser Author { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}

