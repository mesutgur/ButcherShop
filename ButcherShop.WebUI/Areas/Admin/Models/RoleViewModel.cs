using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ButcherShop.WebUI.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Display(Name = "Kullanıcı Sayısı")]
        public int UserCount { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime? CreatedDate { get; set; }
    }

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }
    }

    public class EditRoleViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [Display(Name = "Rol Adı")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Display(Name = "Kullanıcılar")]
        public List<string> Users { get; set; }
    }
}


