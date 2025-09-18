using ShopZone.Data;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopZone.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopZone.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public JwtService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.Phone ?? ""),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("pincode", user.Pincode ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenExpiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "15");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7), // 7 days
                IsRevoked = false
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        public async Task<User?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken
                    && !rt.IsRevoked
                    && rt.ExpiryDate > DateTime.UtcNow);

            return storedToken?.User;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}