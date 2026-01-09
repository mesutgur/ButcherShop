using System;

namespace ButcherShop.Entity.Entities
{
    public class ContactMessage : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public string AdminNote { get; set; }
    }
}