using System.ComponentModel.DataAnnotations;

namespace ButcherShop.WebUI.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [StringLength(100)]
        [Display(Name = "Konu")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur")]
        [StringLength(2000, MinimumLength = 10)]
        [Display(Name = "Mesaj")]
        public string Message { get; set; }
    }
}