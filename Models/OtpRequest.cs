using System.ComponentModel.DataAnnotations;
using ShopZone.Models;

namespace ShopZone.Models
{
    public class OtpRequest : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Identifier { get; set; } = string.Empty; // Email or Phone

        [Required, MaxLength(6)]
        public string Otp { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}