// Path: LedgerLink/ViewModels/CustomerDetailsViewModel.cs
using System.Collections.Generic;
using System.Linq;
using LedgerLink.Models;
using X.PagedList;

namespace LedgerLink.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public Customer Customer { get; set; } = null!;

        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // Use StaticPagedList for safe empty defaults (no need to call ToPagedList here)
        public IPagedList<Transaction> Transactions { get; set; } 
            = new StaticPagedList<Transaction>(Enumerable.Empty<Transaction>(), 1, 10, 0);

        public IPagedList<Payment> Payments { get; set; } 
            = new StaticPagedList<Payment>(Enumerable.Empty<Payment>(), 1, 10, 0);
    }
}
