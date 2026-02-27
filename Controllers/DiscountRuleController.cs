// Path: LedgerLink/Controllers/DiscountRuleController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using LedgerLink.Interface; // For IFestivalRepo, IDiscountRuleRepo
using LedgerLink.Models;   // For Festival, DiscountRule models
using Microsoft.AspNetCore.Http; // For HttpContext.Session
using Microsoft.AspNetCore.Mvc;

namespace LedgerLink.Controllers
{
    public class DiscountRuleController : Controller
    {
        private readonly IDiscountRuleRepo _discountRuleRepo;
        private readonly IFestivalRepo _festivalRepo;

        public DiscountRuleController(IDiscountRuleRepo discountRuleRepo, IFestivalRepo festivalRepo)
        {
            _discountRuleRepo = discountRuleRepo;
            _festivalRepo = festivalRepo;
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

        private Guid GetShopId()
        {
            var shopId = HttpContext.Session.GetString("ShopId");
            if (string.IsNullOrEmpty(shopId) || !Guid.TryParse(shopId, out var parsedShopId))
            {
                throw new InvalidOperationException("ShopId not found in session");
            }
            return parsedShopId;
        }

        // GET: DiscountRule/Index/{festivalId} - Displays all rules for a specific festival
        public IActionResult Index(int festivalId)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Festival? festival = _festivalRepo.GetAllFestivals(shopId).FirstOrDefault(f => f.Id == festivalId);
            if (festival == null)
            {
                return NotFound("Festival not found.");
            }

            ViewBag.FestivalName = festival.Name;
            ViewBag.FestivalId = festival.Id;

            IEnumerable<DiscountRule> rules = _discountRuleRepo.GetAllDiscountRules(shopId)
                                                               .Where(r => r.FestivalId == festivalId)
                                                               .OrderBy(r => r.MinCustomerCreditBalance)
                                                               .ToList();
            return View(rules);
        }

        // GET: DiscountRule/Create/{festivalId} - Displays form to add a new rule
        public IActionResult Create(int festivalId)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            ViewBag.FestivalName = _festivalRepo.GetFestivalById(festivalId, shopId)?.Name;
            ViewBag.FestivalId = festivalId;
            return View();
        }

        // POST: DiscountRule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DiscountRule rule)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                rule.ShopId = GetShopId();
                _discountRuleRepo.AddDiscountRule(rule);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            var shopId = GetShopId();
            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId, shopId)?.Name;
            ViewBag.FestivalId = rule.FestivalId;
            return View(rule);
        }

        // GET: DiscountRule/Edit/{id}
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id, shopId);
            if (rule == null)
            {
                return NotFound();
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId, shopId)?.Name;
            ViewBag.FestivalId = rule.FestivalId;
            return View(rule);
        }

        // POST: DiscountRule/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DiscountRule rule)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Preserve ShopId
                var existingRule = _discountRuleRepo.GetDiscountRuleById(rule.Id, GetShopId());
                if (existingRule != null)
                {
                    rule.ShopId = existingRule.ShopId;
                }
                _discountRuleRepo.UpdateDiscountRule(rule);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            var shopId = GetShopId();
            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId, shopId)?.Name;
            ViewBag.FestivalId = rule.FestivalId;
            return View(rule);
        }
        
        // GET: DiscountRule/Delete/{id}
        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id, shopId);
            if (rule == null)
            {
                return NotFound();
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId, shopId)?.Name;
            ViewBag.FestivalId = rule.FestivalId;
            return View(rule);
        }

        // POST: DiscountRule/DeleteConfirmed
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id, shopId);
            if (rule != null)
            {
                _discountRuleRepo.DeleteDiscountRule(id, shopId);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            return NotFound();
        }
    }
}