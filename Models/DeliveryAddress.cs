using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class DeliveryAddress : BaseEntity
    {
        public int UserId { get; set; }

        [Required, MaxLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}