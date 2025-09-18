using ShopZone.Models;
using ShopZone.Models;

namespace ShopZone.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user);
        Task<User?> ValidateRefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}