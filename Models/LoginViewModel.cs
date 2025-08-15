// Path: LedgerLink/Models/LoginViewModel.cs
using System.ComponentModel.DataAnnotations; // Required for [Required] and [DataType]

namespace LedgerLink.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty; // Non-nullable, initialized

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)] // Hides input in UI
        public string Password { get; set; } = string.Empty; // Non-nullable, initialized
    }
}