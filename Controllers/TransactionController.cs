// Path: LedgerLink/Controllers/TransactionController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LedgerLink.Interface;
using LedgerLink.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IProductRepo _productRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IPaymentRepo _paymentRepo;

        public TransactionController(
            ICustomerRepo customerRepo,
            IProductRepo productRepo,
            ITransactionRepo transactionRepo,
            IPaymentRepo paymentRepo)
        {
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _transactionRepo = transactionRepo;
            _paymentRepo = paymentRepo;
        }

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
        }

        public IActionResult Scan()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Transaction/ProcessScan - Receives the scanned barcode ID (which is Customer.Id)
        [HttpPost]
        public IActionResult ProcessScan([FromBody] string customerIdString) // Parameter name changed for clarity
        {
            if (!IsAdminLoggedIn())
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(customerIdString))
            {
                return BadRequest("Customer ID cannot be empty.");
            }

            // Parse the incoming string to a Guid
            // CRITICAL FIX 1: Use a descriptive name for the out parameter (scannedCustomerId)
            if (!Guid.TryParse(customerIdString.Trim(), out Guid scannedCustomerId))
            {
                return BadRequest("Invalid customer ID format received.");
            }

            // Look up the customer by their Id (which is a Guid)
            Customer? customer = _customerRepo.GetCustomerById(scannedCustomerId); // Use GetCustomerById

            if (customer == null)
            {
                return NotFound("Customer not found for the scanned ID.");
            }

            // CRITICAL FIX 2: Return the customerId property using the correct variable name
            return Ok(new { customerId = customer.Id }); // Ensure 'customerId' property matches what JS expects
        }

        // GET: Transaction/CustomerDetails/{id} - Displays customer info and products for sale
        // CRITICAL FIX 3: Parameter name must be 'id' to match the default route pattern
        public IActionResult CustomerDetails(Guid id) // Parameter name changed to 'id'
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // CRITICAL FIX 4: Use 'id' (the action's parameter) when calling the repository
            Customer? customer = _customerRepo.GetCustomerById(id);
            if (customer == null)
            {
                return NotFound("Customer not found."); // This is the 404 message you're seeing if not found
            }

            IEnumerable<Product> products = _productRepo.GetAllProducts();

            // Load customer's transactions and payments
            // CRITICAL FIX 5: Use 'id' (the action's parameter) in the Where clauses
            IEnumerable<Transaction> customerTransactions = _transactionRepo.GetAllTransactions()
                                                                            .Where(t => t.CustomerId == id)
                                                                            .OrderByDescending(t => t.PurchaseDate)
                                                                            .ToList();
            IEnumerable<Payment> customerPayments = _paymentRepo.GetAllPayments()
                                                                 .Where(p => p.CustomerId == id)
                                                                 .OrderByDescending(p => p.PaymentDate)
                                                                 .ToList();

            var viewModel = new CustomerDetailsViewModel
            {
                Customer = customer,
                Products = products,
                Transactions = customerTransactions,
                Payments = customerPayments
            };

            return View(viewModel);
        }
    }
}