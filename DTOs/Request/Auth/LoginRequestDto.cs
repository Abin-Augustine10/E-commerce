using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Auth
{
    public class LoginRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty; // Email or Phone

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}