using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class RefreshToken : BaseEntity
    {
        public int UserId { get; set; }

        [Required, MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; } = false;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
    }
}