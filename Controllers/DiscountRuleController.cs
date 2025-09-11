// Path: LedgerLink/Controllers/DiscountRuleController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For HttpContext.Session
using LedgerLink.Interface; // For IFestivalRepo, IDiscountRuleRepo
using LedgerLink.Models;   // For Festival, DiscountRule models
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        // --- Manual Session Check for Protection ---
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
        }

        // GET: DiscountRule/Index/{festivalId} - Displays all rules for a specific festival
        public IActionResult Index(int festivalId)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            Festival? festival = _festivalRepo.GetAllFestivals().FirstOrDefault(f => f.Id == festivalId);
            if (festival == null)
            {
                return NotFound("Festival not found.");
            }

            ViewBag.FestivalName = festival.Name;
            ViewBag.FestivalId = festival.Id;

            IEnumerable<DiscountRule> rules = _discountRuleRepo.GetAllDiscountRules()
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

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(festivalId)?.Name;
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
                _discountRuleRepo.AddDiscountRule(rule);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId)?.Name;
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

            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id);
            if (rule == null)
            {
                return NotFound();
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId)?.Name;
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
                _discountRuleRepo.UpdateDiscountRule(rule);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId)?.Name;
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

            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id);
            if (rule == null)
            {
                return NotFound();
            }

            ViewBag.FestivalName = _festivalRepo.GetFestivalById(rule.FestivalId)?.Name;
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

            DiscountRule? rule = _discountRuleRepo.GetDiscountRuleById(id);
            if (rule != null)
            {
                _discountRuleRepo.DeleteDiscountRule(id);
                return RedirectToAction(nameof(Index), new { festivalId = rule.FestivalId });
            }

            return NotFound();
        }
    }
}