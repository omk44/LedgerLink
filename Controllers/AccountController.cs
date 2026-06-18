using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LedgerLink.Interface;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LedgerLink.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IShopRepo _shopRepo;
        
        public AccountController(IAdminRepo adminRepo, IShopRepo shopRepo)
        {
            _adminRepo = adminRepo;
            _shopRepo = shopRepo;
        }

        // Generate secure admin token
        private string GenerateSecureToken(string adminId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var randomString = Convert.ToBase64String(randomBytes);
            return $"ADMIN_{adminId}_{timestamp}_{randomString}";
        }

        // Validate admin token format and expiry
        private bool IsValidAdminToken(string token, out bool isExpired)
        {
            isExpired = false;
            
            if (string.IsNullOrEmpty(token) || !token.StartsWith("ADMIN_"))
                return false;

            var parts = token.Split('_');
            if (parts.Length < 4)
                return false;

            if (long.TryParse(parts[2], out long timestamp))
            {
                var tokenTime = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                var expiry = tokenTime.AddMinutes(30); // 30 minute session
                
                if (DateTime.UtcNow > expiry)
                {
                    isExpired = true;
                    return false;
                }
            }

            return true;
        }

        // Enhanced session validation
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
                ClearSession();
                return false;
            }
            
            // Validate token
            if (IsValidAdminToken(sessionToken, out bool isExpired))
            {
                if (isExpired)
                {
                    ClearSession();
                    return false;
                }
                return true;
            }
            
            return false;
        }

        // Clear all session data
        private void ClearSession()
        {
            HttpContext.Session.Remove("AdminToken");
            HttpContext.Session.Remove("SessionExpiry");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("ShopId");
            HttpContext.Session.Remove("IsAdminLoggedIn");
            HttpContext.Session.Remove("AdminEmail");
            HttpContext.Session.Remove("AdminName");
        }

        // GET: /Account/Login - Displays the login form
        public IActionResult Login()
        {
            // If the admin is already logged in, redirect them to the home page.
            if (IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(); // Return the Login view
        }

        // POST: /Account/Login - Handles login form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Keep users on login page; do not redirect to a separate registration page.
            if (!await _adminRepo.HasAnyAdminAsync())
            {
                ModelState.AddModelError(string.Empty, "No admin account found. Please register your shop first.");
                return View(model);
            }

            // Check if the submitted model data is valid based on data annotations
            if (ModelState.IsValid)
            {
                var admin = await _adminRepo.GetByEmailAsync(model.Username);

                if (admin == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                    return View(model);
                }

                // Check if account is locked
                if (admin.IsLockedOut)
                {
                    var remainingMinutes = (int)(admin.LockoutEnd!.Value - DateTime.UtcNow).TotalMinutes;
                    ModelState.AddModelError(string.Empty, 
                        $"Account is locked due to too many failed attempts. Try again in {remainingMinutes} minutes.");
                    return View(model);
                }

                // Check if account is active
                if (!admin.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "This account has been deactivated.");
                    return View(model);
                }

                // Validate password
                if (BCrypt.Net.BCrypt.Verify(model.Password, admin.PasswordHash))
                {
                    // Successful login - Reset failed attempts
                    await _adminRepo.ResetFailedLoginAsync(admin);

                    // Generate secure session data
                    var adminToken = GenerateSecureToken(admin.Id.ToString());
                    var sessionExpiry = DateTime.UtcNow.AddMinutes(30);

                    // Fetch shop details for session
                    var shop = await _shopRepo.GetByIdAsync(admin.ShopId);

                    // Set secure session variables
                    HttpContext.Session.SetString("AdminToken", adminToken);
                    HttpContext.Session.SetString("SessionExpiry", sessionExpiry.ToString());
                    HttpContext.Session.SetString("UserId", admin.Id.ToString());
                    HttpContext.Session.SetString("ShopId", admin.ShopId.ToString());
                    HttpContext.Session.SetString("AdminEmail", admin.Email);
                    HttpContext.Session.SetString("AdminName", admin.FullName);
                    HttpContext.Session.SetString("ShopName", shop?.ShopName ?? "LedgerLink");
                    HttpContext.Session.SetString("ShopPhoneNumber", shop?.PhoneNumber ?? "");
                    HttpContext.Session.SetString("IsAdminLoggedIn", "true");

                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    // Failed login - Increment failed attempts
                    await _adminRepo.IncrementFailedLoginAsync(admin);

                    var remainingAttempts = 5 - admin.FailedLoginAttempts - 1;
                    
                    if (remainingAttempts > 0)
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"Invalid email or password. {remainingAttempts} attempts remaining.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, 
                            "Too many failed attempts. Account locked for 15 minutes.");
                    }
                }
            }

            // If model state is invalid or login failed, return to the login view with errors
            return View(model);
        }

        // GET: /Account/Logout - Logs out the admin
        public IActionResult Logout()
        {
            // Clear all session data to "log out" the admin
            ClearSession();
            
            return RedirectToAction("Login", "Account");
        }
    }
}
