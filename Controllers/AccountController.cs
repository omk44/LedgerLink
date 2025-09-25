using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using LedgerLink.ViewModels; // Required for LoginViewModel
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;


namespace LedgerLink.Controllers
{
    public class AccountController : Controller
    {
        private readonly AdminSettings _adminSettings;
        
        public AccountController(IOptions<AdminSettings> adminSettings)
        {
            _adminSettings = adminSettings.Value;
        }

        // Generate secure admin token
        private string GenerateSecureToken(string username)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var randomString = Convert.ToBase64String(randomBytes);
            return $"ADMIN_{username}_{timestamp}_{randomString}";
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
                var expiry = tokenTime.AddMinutes(_adminSettings.SessionTimeoutMinutes);
                
                if (DateTime.UtcNow > expiry)
                {
                    isExpired = true;
                    return false;
                }
            }

            return parts[1] == _adminSettings.Username;
        }

        // Check login attempts and lockout
        private bool IsAccountLocked()
        {
            var lockoutEnd = HttpContext.Session.GetString("LockoutEnd");
            if (!string.IsNullOrEmpty(lockoutEnd))
            {
                if (DateTime.TryParse(lockoutEnd, out DateTime lockoutEndTime) && 
                    DateTime.UtcNow < lockoutEndTime)
                {
                    return true;
                }
                else
                {
                    // Lockout period has ended, clear lockout data
                    HttpContext.Session.Remove("LockoutEnd");
                    HttpContext.Session.Remove("LoginAttempts");
                }
            }
            return false;
        }

        // Handle failed login attempts
        private void HandleFailedLogin()
        {
            var attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;
            attempts++;
            HttpContext.Session.SetInt32("LoginAttempts", attempts);

            if (attempts >= _adminSettings.MaxLoginAttempts)
            {
                var lockoutEnd = DateTime.UtcNow.AddMinutes(_adminSettings.LockoutDurationMinutes);
                HttpContext.Session.SetString("LockoutEnd", lockoutEnd.ToString());
            }
        }

        // Clear login attempts on successful login
        private void ClearLoginAttempts()
        {
            HttpContext.Session.Remove("LoginAttempts");
            HttpContext.Session.Remove("LockoutEnd");
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
            HttpContext.Session.Remove("IsAdminLoggedIn");
        }

        // GET: /Account/Login - Displays the login form
        public IActionResult Login()
        {
            // If the admin is already logged in, redirect them to the home page.
            if (IsAdminLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            // Check if account is locked
            if (IsAccountLocked())
            {
                var lockoutEnd = HttpContext.Session.GetString("LockoutEnd");
                if (DateTime.TryParse(lockoutEnd, out DateTime lockoutEndTime))
                {
                    var remainingMinutes = (int)(lockoutEndTime - DateTime.UtcNow).TotalMinutes;
                    ModelState.AddModelError(string.Empty, 
                        $"Account is locked due to too many failed attempts. Try again in {remainingMinutes} minutes.");
                }
            }

            return View(); // Return the Login view
        }

        // POST: /Account/Login - Handles login form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF) attacks
        public IActionResult Login(LoginViewModel model)
        {
            // Check if account is locked
            if (IsAccountLocked())
            {
                var lockoutEnd = HttpContext.Session.GetString("LockoutEnd");
                if (DateTime.TryParse(lockoutEnd, out DateTime lockoutEndTime))
                {
                    var remainingMinutes = (int)(lockoutEndTime - DateTime.UtcNow).TotalMinutes;
                    ModelState.AddModelError(string.Empty, 
                        $"Account is locked due to too many failed attempts. Try again in {remainingMinutes} minutes.");
                }
                return View(model);
            }

            // Check if the submitted model data is valid based on data annotations
            if (ModelState.IsValid)
            {
                // Validate credentials against configuration values
                if (model.Username == _adminSettings.Username && model.Password == _adminSettings.Password)
                {
                    // Clear any previous failed login attempts
                    ClearLoginAttempts();

                    // Generate secure session data
                    var adminToken = GenerateSecureToken(_adminSettings.Username);
                    var sessionExpiry = DateTime.UtcNow.AddMinutes(_adminSettings.SessionTimeoutMinutes);

                    // Set secure session variables
                    HttpContext.Session.SetString("AdminToken", adminToken);
                    HttpContext.Session.SetString("SessionExpiry", sessionExpiry.ToString());
                    HttpContext.Session.SetString("UserId", _adminSettings.Username);
                    HttpContext.Session.SetString("IsAdminLoggedIn", "true");

                    return RedirectToAction("Index", "Home"); // Redirect to your main application page
                }
                
                // Handle failed login
                HandleFailedLogin();
                
                var attempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;
                var remainingAttempts = _adminSettings.MaxLoginAttempts - attempts;
                
                if (remainingAttempts > 0)
                {
                    ModelState.AddModelError(string.Empty, 
                        $"Invalid username or password. {remainingAttempts} attempts remaining.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, 
                        $"Too many failed attempts. Account locked for {_adminSettings.LockoutDurationMinutes} minutes.");
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
            ClearLoginAttempts();
            
            return RedirectToAction("Login", "Account"); // Redirect back to the login page
        }
    }
}