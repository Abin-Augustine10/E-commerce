using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}