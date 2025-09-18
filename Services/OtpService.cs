using ShopZone.Data;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShopZone.Data;
using ShopZone.Models;
using ShopZone.Services.Interfaces;

namespace ShopZone.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public OtpService(AppDbContext context, IEmailService emailService, ISmsService smsService)
        {
            _context = context;
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<string> GenerateOtpAsync(string identifier)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            // Remove any existing unused OTPs for this identifier
            var existingOtps = await _context.OtpRequests
                .Where(o => o.Identifier == identifier && !o.IsUsed)
                .ToListAsync();

            _context.OtpRequests.RemoveRange(existingOtps);

            var otpRequest = new OtpRequest
            {
                Identifier = identifier,
                Otp = otp,
                Expiry = expiry,
                IsUsed = false
            };

            _context.OtpRequests.Add(otpRequest);
            await _context.SaveChangesAsync();

            return otp;
        }

        public async Task<bool> ValidateOtpAsync(string identifier, string otp)
        {
            var otpRequest = await _context.OtpRequests
                .FirstOrDefaultAsync(o => o.Identifier == identifier
                    && o.Otp == otp
                    && !o.IsUsed
                    && o.Expiry > DateTime.UtcNow);

            if (otpRequest != null)
            {
                otpRequest.IsUsed = true;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task SendOtpAsync(string identifier, string otp)
        {
            if (IsEmail(identifier))
            {
                await _emailService.SendEmailAsync(identifier, "OTP Verification", $"Your OTP is: {otp}");
            }
            else
            {
                await _smsService.SendSmsAsync(identifier, $"Your OTP is: {otp}");
            }
        }

        private static bool IsEmail(string input)
        {
            return input.Contains("@");
        }
    }
}