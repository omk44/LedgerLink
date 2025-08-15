// Path: LedgerLink/Controllers/CustomerController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using LedgerLink.Interface; // For ICustomerRepo
using LedgerLink.Models;   // For Customer model
using LedgerLink.Services; // For QrCodeService
using System; // For Guid
using System.Collections.Generic; // For IEnumerable
using System.Linq; // For LINQ methods like FirstOrDefault
// You might need to add this if not already present, though typically included by default
// using Microsoft.AspNetCore.Mvc.ModelBinding; // For [BindNever] - ensure this is in Customer.cs

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

        // GET: Customer/Index - Displays a list of all customers
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            IEnumerable<Customer> customers = _customerRepo.GetAllCustomers();
            return View(customers);
        }

        // GET: Customer/Create - Displays the form to add a new customer
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Customer/Create - Handles the form submission to add a new customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // --- CRITICAL FIX: Remove Barcode from ModelState validation errors ---
            // This tells ModelState to ignore any errors related to Barcode that occurred
            // during model binding because it was missing from the form data.
            ModelState.Remove("Barcode");

            // Generate the unique GUID for the customer's Id and Barcode
            if (customer.Id == Guid.Empty) // Ensure Id is set if it's a new customer
            {
                customer.Id = Guid.NewGuid();
            }
            customer.Barcode = Guid.NewGuid().ToString(); // Generate and assign the unique barcode

            // Now check ModelState.IsValid. The 'Barcode' property now has a value,
            // and any previous validation errors for it have been cleared.
            if (ModelState.IsValid)
            {
                _customerRepo.AddCustomer(customer); // This will save to DB
                return RedirectToAction("ShowQrCode", new { barcode = customer.Barcode });
            }
            // If ModelState.IsValid is false (due to other validation errors on FullName, Email, etc.),
            // the form will be re-rendered with errors.
            return View(customer);
        }

        // GET: Customer/ShowQrCode - Displays the generated QR code for a customer
        public IActionResult ShowQrCode(string barcode)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(barcode))
            {
                return NotFound("Barcode not provided.");
            }

            Customer? customer = _customerRepo.GetCustomerByBarcode(barcode);
            if (customer == null)
            {
                return NotFound("Customer not found for the given barcode.");
            }

            byte[] qrCodeImageBytes = _qrCodeService.GenerateQrCode(customer.Barcode);

            ViewBag.QrCodeBase64 = Convert.ToBase64String(qrCodeImageBytes);
            ViewBag.CustomerName = customer.FullName;
            ViewBag.CustomerBarcode = customer.Barcode;

            return View(customer);
        }

        // GET: Customer/Edit/{id} - Displays the form to edit an existing customer
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

        // POST: Customer/Edit - Handles the form submission to update a customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // For Edit, Barcode should also be removed from ModelState if it's not in the form
            // or if you want to ensure it's not validated from incoming data.
            ModelState.Remove("Barcode");

            if (ModelState.IsValid)
            {
                _customerRepo.UpdateCustomer(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Delete/{id} - Displays a confirmation page before deleting a customer
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

        // POST: Customer/DeleteConfirmed - Handles the actual deletion of a customer
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