using ShopZone.Services.Interfaces;
using ShopZone.Services.Interfaces;

namespace ShopZone.Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // This is a dummy implementation
            // In production, integrate with SMS providers like Twilio, AWS SNS, etc.
            _logger.LogInformation($"SMS sent to {phoneNumber}: {message}");
            await Task.CompletedTask;
        }
    }
}