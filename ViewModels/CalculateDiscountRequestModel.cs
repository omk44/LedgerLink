using System;

namespace LedgerLink.ViewModels
{
    public class CalculateDiscountRequestModel
    {
        public Guid customerId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public int? applyDiscountFestivalId { get; set; } // Festival ID for discount calculation
    }
}