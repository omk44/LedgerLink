// Path: LedgerLink/Models/ReceiptViewModel.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Models
{
    public class ReceiptViewModel
    {
        // General Receipt Information
        public string ShopName { get; set; } = string.Empty; // Will be set from ShopSettings
        public string AppName { get; set; } = string.Empty; // Will be set from ShopSettings
        public DateTime ReceiptDate { get; set; } = DateTime.UtcNow;

        // Customer Information
        public Customer Customer { get; set; } = null!;

        // Transaction Details (if it's a sale receipt)
        public Transaction? Transaction { get; set; }
        public IEnumerable<Transaction> TransactionItems { get; set; } = new List<Transaction>();

        // Payment Details (if it's a payment receipt)
        public Payment? Payment { get; set; }

        // Summary
        public decimal AmountPaidInThisReceipt { get; set; }
        public decimal CustomerNewBalance { get; set; }
        public string ReceiptType { get; set; } = string.Empty;
    }
}