// Path: LedgerLink/Models/ReceiptViewModel.cs
using System; // For DateTime
using System.Collections.Generic; // For IEnumerable

namespace LedgerLink.Models
{
    public class ReceiptViewModel
    {
        // General Receipt Information
        public string ShopName { get; set; } = "LedgerLink Shop"; // Placeholder, could come from admin settings
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        // Customer Information
        public Customer Customer { get; set; } = null!;

        // Transaction Details (if it's a sale receipt)
        public Transaction? Transaction { get; set; } // Nullable, as it might be a payment receipt
        public IEnumerable<Transaction> TransactionItems { get; set; } = new List<Transaction>(); // For multiple items in a single receipt (if you expand to Order concept)

        // Payment Details (if it's a payment receipt)
        public Payment? Payment { get; set; } // Nullable, as it might be a sale receipt

        // Summary
        public decimal AmountPaidInThisReceipt { get; set; } // Total amount paid for this specific receipt (if payment) or total sale amount (if transaction)
        public decimal CustomerNewBalance { get; set; } // Customer's balance AFTER this transaction/payment
        public string ReceiptType { get; set; } = string.Empty; // "Sale" or "Payment"
    }
}