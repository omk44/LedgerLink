using System.Collections.Generic;

namespace LedgerLink.Models
{
    public class CustomerDetailsViewModel
    {
        public Customer Customer { get; set; } = null!;
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Transaction> Transactions { get; set; } = new List<Transaction>();
        public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
    }
}

//it will help to all necessary data to the CustomerDetails.cshtml view.