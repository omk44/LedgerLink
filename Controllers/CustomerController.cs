// Path: LedgerLink/Controllers/CustomerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using LedgerLink.Interface; // For ICustomerRepo
using LedgerLink.Models;   // For Customer model
using LedgerLink.Services; // For QrCodeService
using System; // For Guid
using System.Collections.Generic; // For IEnumerable
using System.Linq; // For LINQ methods like FirstOrDefault
using System.ComponentModel.DataAnnotations; // For model validation attributes

namespace LedgerLink.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly QrCodeService _qrCodeService;

        public CustomerController(ICustomerRepo customerRepo, QrCodeService qrCodeService)
        {
            _customerRepo = customerRepo;
            _qrCodeService = qrCodeService;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
        }

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            IEnumerable<Customer> customers = _customerRepo.GetAllCustomers();
            return View(customers);
        }

        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // CRITICAL FIX: Remove Barcode from ModelState validation errors
            

            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }
            // CRITICAL FIX: Assign a new Guid directly to Barcode

            if (ModelState.IsValid)
            {
                _customerRepo.AddCustomer(customer);
                // CRITICAL FIX: Pass the Guid.ToString() to the ShowQrCode action
                return RedirectToAction("ShowQrCode", new { id = customer.Id.ToString() });
            }
            return View(customer);
        }

        public IActionResult ShowQrCode(string id) // barcode parameter remains string as it's from URL
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Customer Id not provided.");
            }

            // CRITICAL FIX: Parse the incoming string barcode from URL to Guid
            if (!Guid.TryParse(id, out Guid parsedBarcodeGuid))
            {
                return BadRequest("Invalid customer id format.");
            }

            // CRITICAL FIX: Pass the Guid to GetCustomerByBarcode
            Customer? customer = _customerRepo.GetCustomerById(parsedBarcodeGuid);
            if (customer == null)
            {
                return NotFound("Customer not found for the given barcode.");
            }

            // Generate the QR code image bytes using the QrCodeService with Guid.ToString()
            byte[] qrCodeImageBytes = _qrCodeService.GenerateQrCode(customer.Id);

            ViewBag.QrCodeBase64 = Convert.ToBase64String(qrCodeImageBytes);
            ViewBag.CustomerName = customer.FullName;
            ViewBag.CustomerBarcode = customer.Id.ToString(); // Display as string
            return View(customer);
        }

        public IActionResult Edit(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            Customer? customer = _customerRepo.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }



            if (ModelState.IsValid)
            {
                _customerRepo.UpdateCustomer(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public IActionResult Delete(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            Customer? customer = _customerRepo.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            _customerRepo.DeleteCustomer(id);
            return RedirectToAction(nameof(Index));
        }
    }
}