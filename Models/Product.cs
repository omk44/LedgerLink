// Path: LedgerLink/Models/Product.cs
using System.ComponentModel.DataAnnotations; // For data annotations

namespace LedgerLink.Models
{
    public class Product
    {
        // Scalar Property: Primary Key
        public int Id { get; set; }

        // Scalar Property: Product name with strict validation
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        // Scalar Property: Price of the product with range validation
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 100000.00, ErrorMessage = "Price must be between 0.01 and 100000.")]
        public decimal Price { get; set; }

        // Scalar Property: Product description (optional)
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; } // Nullable
    }
}