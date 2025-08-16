// Path: LedgerLink/Models/Customer.cs
using System;
using System.Collections.Generic; // Required for ICollection
using System.ComponentModel.DataAnnotations; // For data annotations
using Microsoft.AspNetCore.Mvc.ModelBinding; // <--- ADD THIS LINE!

namespace LedgerLink.Models
{
    public class Customer
    {
        // Scalar Property: Primary Key
        [Key] // Indicates this property is the primary key
        [Required(ErrorMessage = "Customer ID is required.")]
        public Guid Id { get; set; }

        // Scalar Property: Customer's full name with strict validation
        [Required(ErrorMessage = "Customer name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        // Scalar Property: Customer's phone number with Phone validation
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        // Scalar Property: Customer's email with EmailAddress validation
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        // Scalar Property: Customer's address (optional)
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; } // Nullable

        // Scalar Property: Tracks the current outstanding balance for the customer
        [Range(0.00, 999999999.99, ErrorMessage = "Current Balance must be a non-negative value.")]
        public decimal CurrentBalance { get; set; } = 0.00m; // Default to 0 for new customers

        // Collection Navigation Property: All transactions made by this customer
        public ICollection<Transaction>? Transactions { get; set; } // Nullable if no transactions yet

        // Collection Navigation Property: All payments made by this customer
        public ICollection<Payment>? Payments { get; set; } // Nullable if no payments yet
    }
}