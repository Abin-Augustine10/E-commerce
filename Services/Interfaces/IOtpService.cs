namespace ShopZone.Services.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string identifier);
        Task<bool> ValidateOtpAsync(string identifier, string otp);
        Task SendOtpAsync(string identifier, string otp);
    }
}