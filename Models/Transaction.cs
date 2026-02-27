// Path: LedgerLink/Models/Transaction.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LedgerLink.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        // Foreign Key: Shop
        [Required]
        public Guid ShopId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        //It can be null if the Product is deleted.
        public int? ProductId { get; set; }
        
        // The navigation property MUST be nullable to match the FK.
        public Product? Product { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price at purchase is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Total Amount is required.")]
        [Range(0.01, 10000000.00, ErrorMessage = "Total Amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        public bool IsCreditTransaction { get; set; } = false;

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        // NEW PROPERTIES FOR DISCOUNT TRACKING
        public int? FestivalId { get; set; }
        public Festival? Festival { get; set; }

        [Range(0.00, 100.00, ErrorMessage = "Discount Percentage must be between 0 and 100.")]
        public decimal DiscountPercentage { get; set; } = 0.00m;

        [Range(0.00, 10000000.00, ErrorMessage = "Discount Amount must be non-negative.")]
        public decimal DiscountAmount { get; set; } = 0.00m;

        [Range(0.00, 10000000.00, ErrorMessage = "Final Amount must be non-negative.")]
        public decimal FinalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string QuantityUnit { get; set; } = string.Empty;

        // Navigation property: The shop this transaction belongs to
        public Shop? Shop { get; set; }
    }
}