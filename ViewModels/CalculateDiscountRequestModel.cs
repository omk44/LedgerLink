using System.Collections.Generic;
using LedgerLink.Models;

namespace LedgerLink.ViewModels
{

    public class CalculateDiscountRequestModel
    {
        public Guid customerId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
    }
}