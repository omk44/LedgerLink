// Path: LedgerLink/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using LedgerLink.Models; // Required for LoginViewModel

namespace LedgerLink.Controllers
{
    public class AccountController : Controller
    {
        // --- HARDCODED CREDENTIALS ---
        // REMINDER: This is EXTREMELY INSECURE and for learning/prototyping ONLY.
        // NEVER use hardcoded credentials or this session-based authentication in a production application.
        private const string AdminUsername = "admin";
        private const string AdminPassword = "password";

        // GET: /Account/Login - Displays the login form
        public IActionResult Login()
        {
            // If the admin is already logged in, redirect them to the home page.
            if (HttpContext.Session.GetString("IsAdminLoggedIn") == "true")
            {
                return RedirectToAction("Index", "Home");
            }
            return View(); // Return the Login view
        }

        // POST: /Account/Login - Handles login form submission
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against Cross-Site Request Forgery (CSRF) attacks
        public IActionResult Login(LoginViewModel model)
        {
            // Check if the submitted model data is valid based on data annotations
            if (ModelState.IsValid)
            {
                // Validate credentials against hardcoded values
                if (model.Username == AdminUsername && model.Password == AdminPassword)
                {
                    // "Login" the admin by setting a session variable
                    HttpContext.Session.SetString("IsAdminLoggedIn", "true");
                    return RedirectToAction("Index", "Product"); // Redirect to your main application page
                }
                // If credentials don't match, add an error to ModelState
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }
            // If model state is invalid or login failed, return to the login view with errors
            return View(model);
        }

        // GET: /Account/Logout - Logs out the admin
        public IActionResult Logout()
        {
            // Clear the session variable to "log out" the admin
            HttpContext.Session.Remove("IsAdminLoggedIn");
            return RedirectToAction("Login", "Account"); // Redirect back to the login page
        }
    }
}