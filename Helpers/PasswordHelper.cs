using System.Security.Cryptography;
using System.Text;

namespace ShopZone.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var saltBytes = Encoding.UTF8.GetBytes("YourSaltHere"); // Use a proper salt in production
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Array.Copy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            var hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hashedPassword, StringComparison.Ordinal);
        }
    }
}
