using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ButcherShop.WebUI.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        [Display(Name = "Roller")]
        public List<string> Roles { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        [Display(Name = "Roller")]
        public List<string> SelectedRoles { get; set; }
    }

    public class EditUserViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Display(Name = "Posta Kodu")]
        public string PostalCode { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }

        [Display(Name = "Roller")]
        public List<string> SelectedRoles { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}