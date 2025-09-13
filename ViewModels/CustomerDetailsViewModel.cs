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

        public IPagedList<Transaction> Transactions { get; set; }
            = new StaticPagedList<Transaction>(Enumerable.Empty<Transaction>(), 1, 10, 0);

        public IPagedList<Payment> Payments { get; set; }
            = new StaticPagedList<Payment>(Enumerable.Empty<Payment>(), 1, 10, 0);

        // ✅ Active festivals for this customer
        public IEnumerable<Festival> ActiveFestivals { get; set; } = new List<Festival>();

        // ✅ All discount rules belonging to active festivals
        public IEnumerable<DiscountRule> ActiveDiscountRules { get; set; } = new List<DiscountRule>();
    }
}
