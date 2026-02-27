// Path: LedgerLink/Services/EmailService.cs
using LedgerLink.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LedgerLink.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _logger = logger;
            _smtpHost = configuration["EmailSettings:SmtpHost"]!;
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = configuration["EmailSettings:SmtpUsername"]!;
            _smtpPassword = configuration["EmailSettings:SmtpPassword"]!;
            _senderEmail = configuration["EmailSettings:SenderEmail"]!;
            _senderName = configuration["EmailSettings:SenderName"]!;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrEmpty(_smtpHost) || string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogError("SMTP settings are not configured properly in appsettings.json under the 'EmailSettings' section.");
                return false;
            }

            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_senderName, _senderEmail));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = message,
                    TextBody = message // Fallback to plain text
                };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    // Connect to Gmail's SMTP server
                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);

                    // Authenticate
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);

                    // Send email
                    await client.SendAsync(emailMessage);

                    // Disconnect
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Email sent successfully to {Recipient}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", toEmail);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string userName, string resetUrl)
        {
            var subject = "Password Reset - LedgerLink";
            var message = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #333;'>Password Reset Request</h2>
                        <p>Hello {userName},</p>
                        <p>We received a request to reset your password for your LedgerLink admin account.</p>
                        <p>Click the button below to reset your password:</p>
                        <p style='margin: 30px 0;'>
                            <a href='{resetUrl}' 
                               style='background-color: #007bff; color: white; padding: 12px 24px; 
                                      text-decoration: none; border-radius: 4px; display: inline-block;'>
                                Reset Password
                            </a>
                        </p>
                        <p>Or copy and paste this link into your browser:</p>
                        <p style='color: #666; word-break: break-all;'>{resetUrl}</p>
                        <p style='margin-top: 30px; color: #666; font-size: 14px;'>
                            <strong>Note:</strong> This link will expire in 1 hour.
                        </p>
                        <p style='color: #666; font-size: 14px;'>
                            If you didn't request a password reset, please ignore this email.
                        </p>
                        <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                        <p style='color: #999; font-size: 12px;'>
                            This is an automated email from LedgerLink. Please do not reply.
                        </p>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(toEmail, subject, message);
        }
    }
}
