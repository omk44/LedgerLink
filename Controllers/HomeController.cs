// Path: LedgerLink/Controllers/HomeController.cs
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LedgerLink.Models;
using Microsoft.Extensions.Options; // Required for IOptions
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using Microsoft.Extensions.Logging; // Required for ILogger

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

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
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
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}