using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class ProductImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public bool IsMainImage { get; set; } // Ana görsel mi?
        public int DisplayOrder { get; set; }

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation Property
        public virtual Product Product { get; set; }
    }
}
