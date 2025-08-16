// Path: LedgerLink/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LedgerLink.Interface;
using LedgerLink.Models;
using System.Linq;
using System;
using System.Collections.Generic;

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

        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
        }

        // GET: Dashboard/Index - Displays the main dashboard with optional date filtering
        public IActionResult Index(DateTime? startDate, DateTime? endDate)
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

            // Get Customers with credit/payment activity in the period
            var customerIdsWithActivity = transactionsInPeriod.Select(t => t.CustomerId)
                                                            .Concat(paymentsInPeriod.Select(p => p.CustomerId))
                                                            .Distinct()
                                                            .ToList();
            var customersWithActivityInPeriod = allCustomers
                .Where(c => customerIdsWithActivity.Contains(c.Id))
                .ToList();


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
                CustomersWithActivityInPeriod = customersWithActivityInPeriod.OrderBy(c => c.FullName)
            };

            return View(viewModel);
        }
    }
}