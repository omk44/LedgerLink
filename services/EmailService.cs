// Path: LedgerLink/Services/EmailSmsService.cs
using LedgerLink.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using SendGrid; // <-- ADD THIS
using SendGrid.Helpers.Mail; // <-- ADD THIS

namespace LedgerLink.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
   
        // SendGrid Settings
        private readonly string _sendGridApiKey;
        private readonly string _senderEmail;
        private readonly string _senderName;

        // Twilio SMS Settings (if still using SMS)
        private readonly string _twilioAccountSid;
        private readonly string _twilioAuthToken;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load SendGrid settings
            _sendGridApiKey = _configuration["EmailSettings:SendGridApiKey"] ?? throw new ArgumentNullException("SendGridApiKey not found in configuration.");
            _senderEmail = _configuration["EmailSettings:SenderEmail"] ?? throw new ArgumentNullException("SenderEmail not found in configuration.");
            _senderName = _configuration["EmailSettings:SenderName"] ?? throw new ArgumentNullException("SenderName not found in configuration.");

        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message); // HTML content can be same as plain text for simplicity

            try
            {
                var response = await client.SendEmailAsync(msg);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email via SendGrid: {ex.Message}");
                return false;
            }
        }

       
    }
}