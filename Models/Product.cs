using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class Product : BaseEntity
    {
        public int SellerId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; } = 0;

        // Navigation Properties
        public virtual User Seller { get; set; } = null!;
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}