// Path: LedgerLink/Models/ShopSettings.cs
namespace LedgerLink.Models
{
    public class ShopSettings
    {
        public string ShopName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string ShopEmail { get; set; } = string.Empty; // NEW: Shop's contact email
        public string ShopPhoneNumber { get; set; } = string.Empty; // NEW: Shop's contact phone number
        // public string InstagramUrl { get; set; } = string.Empty; // NEW: Instagram profile URL
        // public string TwitterUrl { get; set; } = string.Empty; // NEW: Twitter profile URL
        // public string LinkedInUrl { get; set; } = string.Empty; // NEW: LinkedIn profile URL
    }
}