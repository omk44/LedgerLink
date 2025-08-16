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
         // NEW ACTION: POST /Transaction/AddItem - To record a product sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(Guid customerId, int productId, int quantity, bool isCreditTransaction, string? notes)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // 1. Get Customer and Product details
            Customer? customer = _customerRepo.GetCustomerById(customerId);
            Product? product = _productRepo.GetProductById(productId);

            if (customer == null || product == null)
            {
                // Redirect back to CustomerDetails with an error message or show a specific error view
                TempData["ErrorMessage"] = "Customer or Product not found.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            // 2. Calculate total amount for this transaction
            decimal totalAmount = product.Price * quantity;

            // 3. Create new Transaction record
            var newTransaction = new Transaction
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price, // Store price at time of purchase
                TotalAmount = totalAmount,
                IsCreditTransaction = isCreditTransaction,
                PurchaseDate = DateTime.UtcNow,
                Notes = notes
            };

            _transactionRepo.AddTransaction(newTransaction); // Save transaction to DB

            // 4. Update Customer's CurrentBalance if it's a credit transaction
            if (isCreditTransaction)
            {
                customer.CurrentBalance += totalAmount;
                _customerRepo.UpdateCustomer(customer); // Update customer balance in DB
            }

            TempData["SuccessMessage"] = "Item added successfully!";
            return RedirectToAction("CustomerDetails", new { id = customerId }); // Redirect back to refresh details
        }

        // NEW ACTION: POST /Transaction/AddPayment - To record a payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPayment(Guid customerId, decimal amountPaid, string paymentMode)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // 1. Get Customer details
            Customer? customer = _customerRepo.GetCustomerById(customerId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found for payment.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            if (amountPaid <= 0)
            {
                TempData["ErrorMessage"] = "Amount paid must be greater than zero.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            // 2. Create new Payment record
            var newPayment = new Payment
            {
                CustomerId = customerId,
                AmountPaid = amountPaid,
                PaymentMode = paymentMode,
                PaymentDate = DateTime.UtcNow
            };

            _paymentRepo.AddPayment(newPayment); // Save payment to DB

            // 3. Update Customer's CurrentBalance (reduce it)
            customer.CurrentBalance -= amountPaid;
            // Ensure balance doesn't go below zero if they overpay (optional, depends on business logic)
            if (customer.CurrentBalance < 0)
            {
                customer.CurrentBalance = 0;
            }
            _customerRepo.UpdateCustomer(customer); // Update customer balance in DB

            TempData["SuccessMessage"] = "Payment recorded successfully!";
            return RedirectToAction("CustomerDetails", new { id = customerId }); // Redirect back to refresh details
        }

    }
}