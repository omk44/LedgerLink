// Path: LedgerLink/Models/Payment.cs
using System;
using System.ComponentModel.DataAnnotations; // For data annotations

namespace LedgerLink.Models
{
    public class Payment
    {
        // Scalar Property: Primary Key
        [Key] // Indicates this property is the primary key
        [Required(ErrorMessage = "Payment ID is required.")]

        public Guid Id { get; set; }
        
        // Scalar Property: Foreign Key to Customer
        [Required]
        public Guid CustomerId { get; set; }

        // Reference Navigation Property: The Customer who made this payment
        public Customer Customer { get; set; } = null!; // Non-nullable

        // Scalar Property: Date and time the payment was made
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow; // Best practice: Use UtcNow

        // Scalar Property: Amount of money paid
        [Required(ErrorMessage = "Amount Paid is required.")]
        [Range(0.01, 10000000.00, ErrorMessage = "Amount Paid must be greater than 0.")]
        public decimal AmountPaid { get; set; }

        // Scalar Property: Method of payment (e.g., Cash, UPI, Card)
        [Required(ErrorMessage = "Payment Mode is required.")]
        [StringLength(50, ErrorMessage = "Payment Mode cannot exceed 50 characters.")]
        public string PaymentMode { get; set; } = string.Empty;
    }
}