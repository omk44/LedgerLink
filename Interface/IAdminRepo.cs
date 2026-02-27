using System;
using System.Threading.Tasks;
using LedgerLink.Models;

namespace LedgerLink.Interface
{
    public interface IAdminRepo
    {
        Task<Admin?> GetByEmailAsync(string email);
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin?> GetByPasswordResetTokenAsync(string token);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> HasAnyAdminAsync();
        Task<Admin> CreateAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task<bool> ValidateCredentialsAsync(string email, string password);
        Task IncrementFailedLoginAsync(Admin admin);
        Task ResetFailedLoginAsync(Admin admin);
        Task SetPasswordResetTokenAsync(Admin admin, string token, DateTime expiry);
        Task ResetPasswordAsync(Admin admin, string newPasswordHash);
    }
}
