// Path: LedgerLink/Services/EmailService.cs
using LedgerLink.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // <-- Add this
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace LedgerLink.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger; // <-- Use ILogger
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        // Inject ILogger along with IConfiguration
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            _sendGridApiKey = configuration["EmailSettings:SendGridApiKey"]!;
            _senderEmail = configuration["EmailSettings:SenderEmail"]!;
            _senderName = configuration["EmailSettings:SenderName"]!;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_sendGridApiKey) || string.IsNullOrEmpty(_senderEmail))
            {
                _logger.LogError("SendGrid API Key or Sender Email is not configured in appsettings.json under the 'EmailSettings' section.");
                return false;
            }

            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            try
            {
                var response = await client.SendEmailAsync(msg);

                // This is the most important part: check the response status
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {Recipient}", toEmail);
                    return true;
                }
                else
                {
                    // Log the actual error message from SendGrid
                    string responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("SendGrid failed to send email. Status Code: {StatusCode}, Response: {Body}", response.StatusCode, responseBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occurred while sending an email to {Recipient}", toEmail);
                return false;
            }
        }
    }
}