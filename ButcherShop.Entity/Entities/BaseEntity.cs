using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    // Tüm entity'lerin miras alacağı temel sınıf
    // abstract: Bu sınıf direkt kullanılmaz, sadece miras alınır
    public abstract class BaseEntity
    {
        // Primary key (her tabloda olacak)
        public int Id { get; set; }

        // Kayıt oluşturma tarihi
        public DateTime CreatedDate { get; set; }

        // Kayıt güncelleme tarihi (nullable - opsiyonel)
        public DateTime? ModifiedDate { get; set; }

        // Aktif/Pasif durumu
        public bool IsActive { get; set; }

        // Soft delete için (gerçekten silmeden işaretleme)
        public bool IsDeleted { get; set; }
 
    }
}
