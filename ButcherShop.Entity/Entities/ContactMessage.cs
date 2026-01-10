using System;
using System.ComponentModel.DataAnnotations;

namespace ButcherShop.Entity.Entities
{
    public class ContactMessage : BaseEntity
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [MaxLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [MaxLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [MaxLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir")]
        public string Phone { get; set; }

        [MaxLength(200, ErrorMessage = "Konu en fazla 200 karakter olabilir")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [MaxLength(2000, ErrorMessage = "Mesaj en fazla 2000 karakter olabilir")]
        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadDate { get; set; }

        [MaxLength(1000, ErrorMessage = "Admin notu en fazla 1000 karakter olabilir")]
        public string AdminNote { get; set; }
    }
}