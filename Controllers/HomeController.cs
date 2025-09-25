// Path: LedgerLink/Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LedgerLink.Models;
using LedgerLink.ViewModels;
using Microsoft.Extensions.Options; // Required for IOptions
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using Microsoft.Extensions.Logging; // Required for ILogger
using System; // Required for DateTime

namespace LedgerLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ShopSettings _shopSettings;

        // CRITICAL FIX: Combine all dependencies into a single constructor
        public HomeController(ILogger<HomeController> logger, IOptions<ShopSettings> shopSettingsOptions)
        {
            _logger = logger;
            _shopSettings = shopSettingsOptions.Value; // Get the ShopSettings instance
        }

        // --- Enhanced Session Security ---
        // More secure session validation with multiple checks
        private bool IsAdminLoggedIn()
        {
            var sessionToken = HttpContext.Session.GetString("AdminToken");
            var sessionExpiry = HttpContext.Session.GetString("SessionExpiry");
            var sessionUserId = HttpContext.Session.GetString("UserId");
            
            // Basic validation
            if (string.IsNullOrEmpty(sessionToken) || 
                string.IsNullOrEmpty(sessionExpiry) ||
                string.IsNullOrEmpty(sessionUserId))
                return false;
            
            // Check session expiry
            if (DateTime.TryParse(sessionExpiry, out var expiry) && expiry < DateTime.UtcNow)
            {
                // Clear expired session
                HttpContext.Session.Clear();
                return false;
            }
            
            // Additional security: validate token format/content
            return IsValidAdminToken(sessionToken);
        }
        
        private bool IsValidAdminToken(string token)
        {
            // Add token validation logic here
            // Could include encryption validation, database lookup, etc.
            return !string.IsNullOrEmpty(token) && token.StartsWith("ADMIN_");
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }
            ViewBag.ShopName = _shopSettings.ShopName; // Pass ShopName to View
            ViewBag.AppName = _shopSettings.AppName;   // Pass AppName to View
            return View();
        }

         public IActionResult Privacy()
        {
            // No login check needed for Privacy page, it should be public
            return View(_shopSettings); // Pass the ShopSettings model directly
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}