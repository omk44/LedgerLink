using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LedgerLink.Models
{
    public class Shop
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ShopName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ShopEmail { get; set; } = string.Empty;

        [Phone]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string SubscriptionPlan { get; set; } = "Free"; // Free, Basic, Premium

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SubscriptionExpiryDate { get; set; }

        // Navigation properties
        public ICollection<Admin> Admins { get; set; } = new List<Admin>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Festival> Festivals { get; set; } = new List<Festival>();
        public ICollection<DiscountRule> DiscountRules { get; set; } = new List<DiscountRule>();
    }
}
