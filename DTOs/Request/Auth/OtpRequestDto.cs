using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Auth
{
    public class OtpRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty; // Email or Phone
    }

    public class VerifyOtpRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Pincode { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }
    }
}