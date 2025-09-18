using System.ComponentModel.DataAnnotations;

namespace ShopZone.DTOs.Request.Auth
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }

    public class ResetPasswordRequestDto
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        public string Otp { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required, Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
