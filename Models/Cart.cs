using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class Cart : BaseEntity
    {
        public int BuyerId { get; set; }
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        // Navigation Properties
        public virtual User Buyer { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}