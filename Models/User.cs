using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty; // Buyer, Seller, DeliveryPartner

        [MaxLength(10)]
        public string? Pincode { get; set; } // For DeliveryPartner only

        // Navigation Properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();
        public virtual ICollection<Order> BuyerOrders { get; set; } = new List<Order>();
        public virtual ICollection<Order> DeliveryPartnerOrders { get; set; } = new List<Order>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}