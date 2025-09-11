// Path: LedgerLink/Controllers/FestivalController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LedgerLink.Interface;
using LedgerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
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

            if (ModelState.IsValid)
            {
                // CRITICAL FIX: Convert StartDate and EndDate to UTC before saving.
                // The incoming DateTime from the form has an Unspecified Kind, which PostgreSQL rejects.
                festival.StartDate = DateTime.SpecifyKind(festival.StartDate, DateTimeKind.Utc);
                festival.EndDate = DateTime.SpecifyKind(festival.EndDate, DateTimeKind.Utc);

                _festivalRepo.AddFestival(festival);
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

            if (ModelState.IsValid)
            {
                // CRITICAL FIX: Convert StartDate and EndDate to UTC before saving.
                festival.StartDate = DateTime.SpecifyKind(festival.StartDate, DateTimeKind.Utc);
                festival.EndDate = DateTime.SpecifyKind(festival.EndDate, DateTimeKind.Utc);

                _festivalRepo.UpdateFestival(festival);
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