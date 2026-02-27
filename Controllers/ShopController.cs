using System;
using System.Threading.Tasks;
using LedgerLink.Interface;
using LedgerLink.Models;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LedgerLink.Controllers
{
    public class ShopController : Controller
    {
        private readonly IShopRepo _shopRepo;
        private readonly IAdminRepo _adminRepo;

        public ShopController(IShopRepo shopRepo, IAdminRepo adminRepo)
        {
            _shopRepo = shopRepo;
            _adminRepo = adminRepo;
        }

        // GET: /Shop/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Shop/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ShopRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Check if shop email already exists
                var existingShopAdmin = await _adminRepo.GetByEmailAsync(model.ShopEmail);
                if (existingShopAdmin != null)
                {
                    ModelState.AddModelError("ShopEmail", "A shop with this email already exists.");
                    return View(model);
                }

                // Check if admin email already exists
                var existingAdminByEmail = await _adminRepo.GetByEmailAsync(model.AdminEmail);
                if (existingAdminByEmail != null)
                {
                    ModelState.AddModelError("AdminEmail", "An admin with this email already exists.");
                    return View(model);
                }

                // Create new shop
                var shop = new Shop
                {
                    Id = Guid.NewGuid(),
                    ShopName = model.ShopName,
                    ShopEmail = model.ShopEmail,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    SubscriptionPlan = "Free", // Default to free plan
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionExpiryDate = DateTime.UtcNow.AddDays(30) // 30-day free trial
                };

                await _shopRepo.CreateAsync(shop);

                // Create admin for the shop
                var admin = new Admin
                {
                    Id = Guid.NewGuid(),
                    Email = model.AdminEmail,
                    FullName = model.AdminName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    ShopId = shop.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Role = "Owner",
                    FailedLoginAttempts = 0
                };

                await _adminRepo.CreateAsync(admin);

                TempData["SuccessMessage"] = "Shop registered successfully! You can now login with your admin credentials.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }
    }
}
