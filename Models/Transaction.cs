// Path: LedgerLink/Models/Transaction.cs
using System;
using System.ComponentModel.DataAnnotations; // For data annotations

namespace LedgerLink.Models
{
    public class Transaction
    {
        // Scalar Property: Primary Key
        public int Id { get; set; }

        // Scalar Property: Foreign Key to Customer
        [Required]
        public int CustomerId { get; set; }

        // Reference Navigation Property: The Customer who made this transaction
        public Customer Customer { get; set; } = null!; // Non-nullable, EF Core will load this

        // Scalar Property: Foreign Key to Product
        [Required]
        public int ProductId { get; set; }

        // Reference Navigation Property: The Product involved in this transaction
        public Product Product { get; set; } = null!; // Non-nullable

        // Scalar Property: Quantity of the product purchased
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        // Scalar Property: Price of a single unit at the time of purchase
        // Important for historical accuracy if product prices change
        [Required(ErrorMessage = "Unit Price at purchase is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        // Scalar Property: Total amount for this transaction (Quantity * UnitPrice)
        // Store this to avoid recalculation and for exact historical record
        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0.01, 10000000.00, ErrorMessage = "Total Amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        // Scalar Property: Crucial flag - true if this transaction adds to customer's credit
        public bool IsCreditTransaction { get; set; } = false; // Default to a paid transaction

        // Scalar Property: Date and time of the purchase
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow; // Best practice: Use UtcNow

        // Scalar Property: Optional notes for the transaction
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; } // Nullable
    }
}