// Path: LedgerLink/Controllers/FestivalController.cs
using System;
using System.Collections.Generic;
using LedgerLink.Interface;
using LedgerLink.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LedgerLink.Controllers
{
    public class FestivalController : Controller
    {
        private readonly IFestivalRepo _festivalRepo;
        private readonly IDiscountRuleRepo _discountRuleRepo;

        public FestivalController(IFestivalRepo festivalRepo, IDiscountRuleRepo discountRuleRepo)
        {
            _festivalRepo = festivalRepo;
            _discountRuleRepo = discountRuleRepo;
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
            // Enhanced token validation
            if (string.IsNullOrEmpty(token) || !token.StartsWith("ADMIN_"))
                return false;

            try
            {
                var parts = token.Split('_');
                if (parts.Length < 4)
                    return false;

                // Validate timestamp part
                if (long.TryParse(parts[2], out long timestamp))
                {
                    var tokenTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                    var maxAge = TimeSpan.FromHours(24); // Token max age
                    
                    if (DateTime.UtcNow - tokenTime > maxAge)
                        return false; // Token too old
                }
                else
                {
                    return false; // Invalid timestamp
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // GET: Festival/Index
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            IEnumerable<Festival> festivals = _festivalRepo.GetAllFestivals();
            return View(festivals);
        }

        // GET: Festival/Create
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Festival/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Festival festival)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Custom validation: Start date cannot be in the past
            if (festival.StartDate.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError("StartDate", "Festival start date cannot be in the past.");
            }

            // Custom validation: End date must be after start date
            if (festival.EndDate.Date < festival.StartDate.Date)
            {
                ModelState.AddModelError("EndDate", "Festival end date must be after the start date.");
            }

            if (ModelState.IsValid)
            {
                // CRITICAL FIX: Convert StartDate and EndDate to UTC before saving.
                // The incoming DateTime from the form has an Unspecified Kind, which PostgreSQL rejects.
                festival.StartDate = DateTime.SpecifyKind(festival.StartDate, DateTimeKind.Utc);
                festival.EndDate = DateTime.SpecifyKind(festival.EndDate, DateTimeKind.Utc);

                _festivalRepo.AddFestival(festival);
                TempData["SuccessMessage"] = $"Festival '{festival.Name}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(festival);
        }

        // GET: Festival/Edit/{id}
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            Festival? festival = _festivalRepo.GetFestivalById(id);
            if (festival == null)
            {
                return NotFound();
            }
            return View(festival);
        }

        // POST: Festival/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Festival festival)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the existing festival to check if it has already started
            var existingFestival = _festivalRepo.GetFestivalById(festival.Id);
            if (existingFestival == null)
            {
                return NotFound();
            }

            // Custom validation: If festival hasn't started yet, don't allow past start dates
            if (existingFestival.StartDate.Date > DateTime.UtcNow.Date && festival.StartDate.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError("StartDate", "Cannot change festival start date to a past date.");
            }

            // Custom validation: End date must be after start date
            if (festival.EndDate.Date < festival.StartDate.Date)
            {
                ModelState.AddModelError("EndDate", "Festival end date must be after the start date.");
            }

            // Custom validation: Cannot move start date to past if festival is currently active
            if (existingFestival.StartDate.Date <= DateTime.UtcNow.Date && festival.StartDate.Date != existingFestival.StartDate.Date)
            {
                ModelState.AddModelError("StartDate", "Cannot modify start date of an active or completed festival.");
            }

            if (ModelState.IsValid)
            {
                // CRITICAL FIX: Convert StartDate and EndDate to UTC before saving.
                festival.StartDate = DateTime.SpecifyKind(festival.StartDate, DateTimeKind.Utc);
                festival.EndDate = DateTime.SpecifyKind(festival.EndDate, DateTimeKind.Utc);

                _festivalRepo.UpdateFestival(festival);
                TempData["SuccessMessage"] = $"Festival '{festival.Name}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(festival);
        }

        // GET: Festival/Delete/{id}
        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            Festival? festival = _festivalRepo.GetFestivalById(id);
            if (festival == null)
            {
                return NotFound();
            }
            return View(festival);
        }

        // POST: Festival/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            _festivalRepo.DeleteFestival(id);
            return RedirectToAction(nameof(Index));
        }
    }
}