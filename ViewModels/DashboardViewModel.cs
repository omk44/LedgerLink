// Path: LedgerLink/Models/DashboardViewModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using LedgerLink.Models;


namespace LedgerLink.ViewModels
{
    public class DashboardViewModel
    {
        // Date Range Properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Summary Metrics (now filtered by date range)
        public decimal TotalOutstandingCredit { get; set; } // This remains overall, not date-filtered
        public int TotalCustomers { get; set; } // This remains overall, not date-filtered
        public int TotalProducts { get; set; } // This remains overall, not date-filtered
        public decimal TotalSalesInPeriod { get; set; }
        public decimal TotalPaymentsInPeriod { get; set; }

        // Lists for Top/Recent Data
        public IEnumerable<Customer> TopCustomersByCredit { get; set; } = new List<Customer>(); // Overall, top N
        public IEnumerable<Transaction> TransactionsInPeriod { get; set; } = new List<Transaction>();
        public IEnumerable<Payment> PaymentsInPeriod { get; set; } = new List<Payment>();

        public IEnumerable<Customer> CustomersWithActivityInPeriod { get; set; } = new List<Customer>();

        // --- NEW: All Customers with their Current Balances ---
        public IEnumerable<Customer> AllCustomersWithCredit { get; set; } = new List<Customer>();
    }
}