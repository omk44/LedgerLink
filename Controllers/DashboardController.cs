using System;
using System.Linq;
using LedgerLink.Interface;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace LedgerLink.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IProductRepo _productRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IPaymentRepo _paymentRepo;

        public DashboardController(
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

        // GET: Dashboard/Index - Displays the main dashboard with optional date filtering and pagination
        public IActionResult Index(DateTime? startDate, DateTime? endDate, int transactionPage = 1, int paymentPage = 1)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // Set default date range if not provided
            DateTime periodStartDate = startDate?.Date ?? DateTime.UtcNow.Date.AddDays(-30); // Default to last 30 days
            DateTime periodEndDate = endDate?.Date ?? DateTime.UtcNow.Date; // Default to today

            // Ensure end date includes the entire day
            periodEndDate = periodEndDate.Date.AddDays(1).AddTicks(-1); // End of today

            // Get all data (for filtering and overall counts)
            var allCustomers = _customerRepo.GetAllCustomers().ToList();
            var allProducts = _productRepo.GetAllProducts().ToList();
            var allTransactions = _transactionRepo.GetAllTransactions().ToList();
            var allPayments = _paymentRepo.GetAllPayments().ToList();

            // Filter data by date range
            var transactionsInPeriod = allTransactions
                .Where(t => t.PurchaseDate >= periodStartDate && t.PurchaseDate <= periodEndDate)
                .ToList();
            var paymentsInPeriod = allPayments
                .Where(p => p.PaymentDate >= periodStartDate && p.PaymentDate <= periodEndDate)
                .ToList();

            // Calculate Metrics
            decimal totalOutstandingCredit = allCustomers.Sum(c => c.CurrentBalance); // Overall balance
            int totalCustomers = allCustomers.Count(); // Overall count
            int totalProducts = allProducts.Count(); // Overall count
            decimal totalSalesInPeriod = transactionsInPeriod.Sum(t => t.TotalAmount);
            decimal totalPaymentsInPeriod = paymentsInPeriod.Sum(p => p.AmountPaid);

            // Get Top Customers by Credit (Overall, not date-filtered)
            var topCustomersByCredit = allCustomers
                .OrderByDescending(c => c.CurrentBalance)
                .Take(5)
                .ToList();

            // Get Customers with credit activity in the period
            var customerIdsWithActivity = transactionsInPeriod.Select(t => t.CustomerId)
                                                            .Concat(paymentsInPeriod.Select(p => p.CustomerId))
                                                            .Distinct()
                                                            .ToList();
            var customersWithActivityInPeriod = allCustomers
                .Where(c => customerIdsWithActivity.Contains(c.Id))
                .ToList();

            // --- NEW: Get all customers with their current credit, ordered by balance ---
            var allCustomersWithCredit = allCustomers
                .OrderByDescending(c => c.CurrentBalance)
                .ToList();

            // --- NEW: Create paginated collections for all transactions and payments ---
            var allTransactionsPaged = allTransactions
                .OrderByDescending(t => t.PurchaseDate)
                .ToPagedList(transactionPage, 10);

            var allPaymentsPaged = allPayments
                .OrderByDescending(p => p.PaymentDate)
                .ToPagedList(paymentPage, 10);

            // Set ViewBag for pagination
            ViewBag.TransactionPage = transactionPage;
            ViewBag.PaymentPage = paymentPage;

            // Populate ViewModel
            var viewModel = new DashboardViewModel
            {
                StartDate = periodStartDate,
                EndDate = periodEndDate,
                TotalOutstandingCredit = totalOutstandingCredit,
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                TotalSalesInPeriod = totalSalesInPeriod,
                TotalPaymentsInPeriod = totalPaymentsInPeriod,
                TopCustomersByCredit = topCustomersByCredit,
                TransactionsInPeriod = transactionsInPeriod.OrderByDescending(t => t.PurchaseDate).Take(10), // Limit for display
                PaymentsInPeriod = paymentsInPeriod.OrderByDescending(p => p.PaymentDate).Take(10), // Limit for display
                CustomersWithActivityInPeriod = customersWithActivityInPeriod.OrderBy(c => c.FullName),
                AllCustomersWithCredit = allCustomersWithCredit,
                AllTransactionsPaged = allTransactionsPaged,
                AllPaymentsPaged = allPaymentsPaged
            };

            return View(viewModel);
        }
    }
}