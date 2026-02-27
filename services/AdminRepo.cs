using System;
using System.Linq;
using System.Threading.Tasks;
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;
using Microsoft.EntityFrameworkCore;

namespace LedgerLink.Services
{
    public class AdminRepo : IAdminRepo
    {
        private readonly AppDbContext _context;

        public AdminRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.PasswordResetToken == token 
                    && a.PasswordResetTokenExpiry > DateTime.UtcNow);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Admins
                .AnyAsync(a => a.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> HasAnyAdminAsync()
        {
            return await _context.Admins.AnyAsync();
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var admin = await GetByEmailAsync(email);
            if (admin == null || !admin.IsActive || admin.IsLockedOut)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
        }

        public async Task IncrementFailedLoginAsync(Admin admin)
        {
            admin.FailedLoginAttempts++;
            
            // Lock account after 5 failed attempts
            if (admin.FailedLoginAttempts >= 5)
            {
                admin.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            await UpdateAsync(admin);
        }

        public async Task ResetFailedLoginAsync(Admin admin)
        {
            admin.FailedLoginAttempts = 0;
            admin.LockoutEnd = null;
            admin.LastLoginAt = DateTime.UtcNow;
            await UpdateAsync(admin);
        }

        public async Task SetPasswordResetTokenAsync(Admin admin, string token, DateTime expiry)
        {
            admin.PasswordResetToken = token;
            admin.PasswordResetTokenExpiry = expiry;
            await UpdateAsync(admin);
        }

        public async Task ResetPasswordAsync(Admin admin, string newPasswordHash)
        {
            admin.PasswordHash = newPasswordHash;
            admin.PasswordResetToken = null;
            admin.PasswordResetTokenExpiry = null;
            admin.FailedLoginAttempts = 0;
            admin.LockoutEnd = null;
            await UpdateAsync(admin);
        }
    }
}
