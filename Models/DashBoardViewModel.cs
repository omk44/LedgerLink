// Path: LedgerLink/Models/DashboardViewModel.cs
using System; // Required for DateTime
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Models
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
        public decimal TotalSalesInPeriod { get; set; } // Renamed from TotalSalesToday
        public decimal TotalPaymentsInPeriod { get; set; } // Renamed from TotalPaymentsToday

        // Lists for Top/Recent Data (now filtered by date range or overall)
        public IEnumerable<Customer> TopCustomersByCredit { get; set; } = new List<Customer>(); // Overall
        public IEnumerable<Transaction> TransactionsInPeriod { get; set; } = new List<Transaction>(); // Renamed
        public IEnumerable<Payment> PaymentsInPeriod { get; set; } = new List<Payment>(); // Renamed

        // New: Customers with credit activity in the period
        public IEnumerable<Customer> CustomersWithActivityInPeriod { get; set; } = new List<Customer>();
    }
}