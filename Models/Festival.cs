// Path: LedgerLink/Models/Festival.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LedgerLink.Models
{
    /// <summary>
    /// Represents a festival or special date with a defined duration, during which discounts can be applied.
    /// </summary>
    public class Festival
    {
        public int Id { get; set; } // Primary Key

        // Foreign Key: Shop
        [Required]
        public Guid ShopId { get; set; }

        [Required(ErrorMessage = "Festival name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = false;

        // Collection Navigation Property to link to DiscountRules
        public ICollection<DiscountRule>? DiscountRules { get; set; }

        // Navigation property: The shop this festival belongs to
        public Shop? Shop { get; set; }
    }
}