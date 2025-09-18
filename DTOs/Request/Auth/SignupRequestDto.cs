using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Auth
{
    public class SignupRequestDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Identifier { get; set; } = string.Empty; // Email or Phone

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // Buyer, Seller, DeliveryPartner

        [MaxLength(10)]
        public string? Pincode { get; set; } // Required for DeliveryPartner
    }
}