using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ButcherShop.Entity.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }

        // Navigation Properties
        public virtual ICollection<Recipe> Recipes { get; set; }

        // Identity için gerekli metod
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note: authenticationType, CookieAuthenticationOptions.AuthenticationType içinde tanımlanan ile eşleşmelidir
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Özel kullanıcı claim'leri buraya eklenebilir
            // Örnek: userIdentity.AddClaim(new Claim("FullName", FirstName + " " + LastName));

            return userIdentity;
        }
    }
}
