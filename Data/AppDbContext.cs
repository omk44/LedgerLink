
using Microsoft.EntityFrameworkCore; 
using LedgerLink.Models; 

namespace LedgerLink.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shop> Shops { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        public DbSet<Festival> Festivals { get; set; } = null!;
        public DbSet<DiscountRule> DiscountRules { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Configure Primary Keys ---
            modelBuilder.Entity<Customer>()
                .Property(c => c.CurrentBalance)
                .HasColumnType("numeric(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("numeric(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.UnitPrice)
                .HasColumnType("numeric(18, 2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.TotalAmount)
                .HasColumnType("numeric(18, 2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.AmountPaid)
                .HasColumnType("numeric(18, 2)");

            // --- Configure Relationships (Fluent API) ---
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Customer)
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Payments)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany<Transaction>()
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DiscountRule>()
                .Property(r => r.DiscountPercentage)
                .HasColumnType("numeric(5, 2)");
            modelBuilder.Entity<DiscountRule>()
                .Property(r => r.MinCustomerCreditBalance)
                .HasColumnType("numeric(18, 2)");
            modelBuilder.Entity<DiscountRule>()
                .Property(r => r.MaxCustomerCreditBalance)
                .HasColumnType("numeric(18, 2)");
            modelBuilder.Entity<DiscountRule>()
                .Property(r => r.MinPurchaseAmount)
                .HasColumnType("numeric(18, 2)");

            modelBuilder.Entity<Festival>()
            .HasMany(f => f.DiscountRules) // A Festival has many DiscountRules
            .WithOne(r => r.Festival)      // Each DiscountRule belongs to one Festival
            .HasForeignKey(r => r.FestivalId) // Foreign key in DiscountRule table
            .OnDelete(DeleteBehavior.Cascade); // If a Festival is deleted, delete its rules too

            modelBuilder.Entity<Festival>()
                .HasMany<Transaction>()
                .WithOne(t => t.Festival)
                .HasForeignKey(t => t.FestivalId)
                .OnDelete(DeleteBehavior.SetNull); // <-- ADDED this rule

            // --- Multi-Tenant Shop Relationships ---
            // Shop -> Admins (Cascade: deleting shop deletes all admins)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Admins)
                .WithOne(a => a.Shop)
                .HasForeignKey(a => a.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> Customers (Cascade: deleting shop deletes all customers)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Customers)
                .WithOne(c => c.Shop)
                .HasForeignKey(c => c.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> Products (Cascade: deleting shop deletes all products)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Shop)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> Transactions (Cascade: deleting shop deletes all transactions)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Transactions)
                .WithOne(t => t.Shop)
                .HasForeignKey(t => t.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> Payments (Cascade: deleting shop deletes all payments)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Payments)
                .WithOne(p => p.Shop)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> Festivals (Cascade: deleting shop deletes all festivals)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Festivals)
                .WithOne(f => f.Shop)
                .HasForeignKey(f => f.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shop -> DiscountRules (Cascade: deleting shop deletes all discount rules)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.DiscountRules)
                .WithOne(r => r.Shop)
                .HasForeignKey(r => r.ShopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}