using ShopZone.Services.Interfaces;
using ShopZone.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ShopZone.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");

            using var client = new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]!),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]!)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["Username"]!, smtpSettings["DisplayName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}