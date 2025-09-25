// Path: LedgerLink/ViewModels/AdminSettings.cs
using System.ComponentModel.DataAnnotations;

namespace LedgerLink.ViewModels
{
    public class AdminSettings
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public int SessionTimeoutMinutes { get; set; } = 30;
        
        public int MaxLoginAttempts { get; set; } = 5;
        
        public int LockoutDurationMinutes { get; set; } = 15;
        
        public bool RequireSecureToken { get; set; } = true;
    }
}