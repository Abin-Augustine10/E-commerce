namespace ShopZone.DTOs.Response.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Pincode { get; set; }
    }
}