using System.ComponentModel.DataAnnotations;

namespace LedgerLink.ViewModels
{
    public class ShopRegistrationViewModel
    {
        [Required(ErrorMessage = "Shop name is required")]
        [StringLength(100)]
        [Display(Name = "Shop Name")]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shop email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Shop Email")]
        public string ShopEmail { get; set; } = string.Empty;

        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        [Display(Name = "Shop Address")]
        public string? Address { get; set; }

        // Admin Details
        [Required(ErrorMessage = "Admin name is required")]
        [StringLength(100)]
        [Display(Name = "Admin Full Name")]
        public string AdminName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        [Display(Name = "Admin Email")]
        public string AdminEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
