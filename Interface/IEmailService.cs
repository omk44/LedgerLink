// Path: LedgerLink/Interface/IEmailSmsService.cs
using System.Threading.Tasks;

namespace LedgerLink.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string message);
    }
}

