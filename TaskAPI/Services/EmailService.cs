using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskAPI.Interfaces;
using TaskAPI.Models;

namespace TaskAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpConfig _smtpConfig;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpConfig> smtpConfig, ILogger<EmailService> logger)
        {
            _smtpConfig = smtpConfig.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var client = new SmtpClient(_smtpConfig.Host, _smtpConfig.Port)
            {
                Credentials = new NetworkCredential(_smtpConfig.Username, _smtpConfig.Password),
                EnableSsl = _smtpConfig.EnableSsl
            };

            var mailMessage = new MailMessage(_smtpConfig.Username, email, subject, message);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }
    }
}