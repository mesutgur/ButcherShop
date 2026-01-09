using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButcherShop.Entity.Entities
{
    public class RecipeProduct : BaseEntity
    {
        // Bir tarifin hangi ürünleri kullandığını ve miktarını tutar.
        public int RecipeId { get; set; }
        public int ProductId { get; set; }
        public string Quantity { get; set; } // Miktar: "500 gr", "1 kg" vb.

        // Navigation Properties
        public virtual Recipe Recipe { get; set; }
        public virtual Product Product { get; set; }
    }
}
