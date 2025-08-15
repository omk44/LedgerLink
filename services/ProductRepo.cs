// Path: LedgerLink/Services/ProductRepo.cs
using System.Collections.Generic;
using System.Linq;
using LedgerLink.Data; // Required to inject AppDbContext
using LedgerLink.Interface;
using LedgerLink.Models;

namespace LedgerLink.Services
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.Find(id);
        }
        
        public Product AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product? UpdateProduct(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                _context.SaveChanges();
            }
            return existingProduct;
        }

        public Product? DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return product;
        }
    }
}