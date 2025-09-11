// Path: LedgerLink/Models/DiscountRule.cs
using System.ComponentModel.DataAnnotations;

namespace LedgerLink.Models
{
    /// <summary>
    /// Defines a discount rule that applies to a specific festival and customer credit range.
    /// </summary>
    public class DiscountRule
    {
        public int Id { get; set; } // Primary Key

        [Required]
        public int FestivalId { get; set; } // Foreign Key to Festival
        public Festival? Festival { get; set; } = null!; // Reference Navigation Property

        [Required(ErrorMessage = "Discount percentage is required.")]
        [Range(0, 100.00, ErrorMessage = "Discount must be between 0.01 and 100.")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Minimum customer credit balance is required.")]
        [Range(0.00, 999999999.99)]
        public decimal MinCustomerCreditBalance { get; set; }

        [Required(ErrorMessage = "Maximum customer credit balance is required.")]
        [Range(0.00, 999999999.99)]
        public decimal MaxCustomerCreditBalance { get; set; }

        [Required(ErrorMessage = "Minimum purchase amount is required.")]
        [Range(0.01, 999999999.99)]
        public decimal MinPurchaseAmount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}