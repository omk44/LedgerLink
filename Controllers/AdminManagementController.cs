using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LedgerLink.Interface;
using LedgerLink.Models;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerLink.Controllers
{
    public class AdminManagementController : Controller
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IEmailService _emailService;

        public AdminManagementController(IAdminRepo adminRepo, IEmailService emailService)
        {
            _adminRepo = adminRepo;
            _emailService = emailService;
        }

        // GET: /AdminManagement/Register
        // Only show registration if no admin exists
        public async Task<IActionResult> Register()
        {
            // Check if any admin already exists
            if (await _adminRepo.HasAnyAdminAsync())
            {
                TempData["Error"] = "Admin account already exists. Please login.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: /AdminManagement/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminRegisterViewModel model)
        {
            // Check if any admin already exists
            if (await _adminRepo.HasAnyAdminAsync())
            {
                TempData["Error"] = "Admin account already exists. Please login.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _adminRepo.EmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                // Create new admin
                var admin = new Admin
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _adminRepo.CreateAsync(admin);

                TempData["Success"] = "Admin account created successfully! Please login.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // GET: /AdminManagement/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /AdminManagement/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _adminRepo.GetByEmailAsync(model.Email);

                if (admin != null && admin.IsActive)
                {
                    // Generate password reset token
                    var token = GenerateSecureToken();
                    var expiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

                    await _adminRepo.SetPasswordResetTokenAsync(admin, token, expiry);

                    // Generate reset URL
                    var resetUrl = Url.Action("ResetPassword", "AdminManagement", 
                        new { token = token, email = admin.Email }, Request.Scheme);

                    // Send email
                    var emailSent = await _emailService.SendPasswordResetEmailAsync(
                        admin.Email, 
                        admin.FullName, 
                        resetUrl ?? string.Empty);

                    if (emailSent)
                    {
                        TempData["Success"] = "Password reset instructions have been sent to your email.";
                    }
                    else
                    {
                        TempData["Error"] = "Failed to send password reset email. Please try again.";
                        return View(model);
                    }
                }
                else
                {
                    // Don't reveal that the email doesn't exist (security best practice)
                    TempData["Success"] = "If an account with that email exists, password reset instructions have been sent.";
                }

                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // GET: /AdminManagement/ResetPassword
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid password reset link.";
                return RedirectToAction("Login", "Account");
            }

            var admin = await _adminRepo.GetByPasswordResetTokenAsync(token);
            
            if (admin == null || admin.Email.ToLower() != email.ToLower())
            {
                TempData["Error"] = "Invalid or expired password reset link.";
                return RedirectToAction("Login", "Account");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        // POST: /AdminManagement/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _adminRepo.GetByPasswordResetTokenAsync(model.Token);

                if (admin == null || admin.Email.ToLower() != model.Email.ToLower())
                {
                    TempData["Error"] = "Invalid or expired password reset link.";
                    return RedirectToAction("Login", "Account");
                }

                // Hash new password and reset
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                await _adminRepo.ResetPasswordAsync(admin, newPasswordHash);

                TempData["Success"] = "Your password has been reset successfully! Please login.";
                return RedirectToAction("Login", "Account");
            }

            return View(model);
        }

        // Helper method to generate secure token
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
