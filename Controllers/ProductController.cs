// Path: LedgerLink/Controllers/ProductController.cs
using System;
using System.Collections.Generic;
using LedgerLink.Interface; 
using LedgerLink.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace LedgerLink.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo;

        public ProductController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        // --- Enhanced Session Security ---
        // More secure session validation with multiple checks
        private bool IsAdminLoggedIn()
        {
            var sessionToken = HttpContext.Session.GetString("AdminToken");
            var sessionExpiry = HttpContext.Session.GetString("SessionExpiry");
            var sessionUserId = HttpContext.Session.GetString("UserId");
            
            // Basic validation
            if (string.IsNullOrEmpty(sessionToken) || 
                string.IsNullOrEmpty(sessionExpiry) ||
                string.IsNullOrEmpty(sessionUserId))
                return false;
            
            // Check session expiry
            if (DateTime.TryParse(sessionExpiry, out var expiry) && expiry < DateTime.UtcNow)
            {
                // Clear expired session
                HttpContext.Session.Clear();
                return false;
            }
            
            // Additional security: validate token format/content
            return IsValidAdminToken(sessionToken);
        }
        
        private bool IsValidAdminToken(string token)
        {
            // Add token validation logic here
            // Could include encryption validation, database lookup, etc.
            return !string.IsNullOrEmpty(token) && token.StartsWith("ADMIN_");
        }

        private Guid GetShopId()
        {
            var shopId = HttpContext.Session.GetString("ShopId");
            if (string.IsNullOrEmpty(shopId) || !Guid.TryParse(shopId, out var parsedShopId))
            {
                throw new InvalidOperationException("ShopId not found in session");
            }
            return parsedShopId;
        }

        // GET: Product/Index - Displays a list of all products
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }
            var shopId = GetShopId();
            IEnumerable<Product> products = _productRepo.GetAllProducts(shopId);
            return View(products);
        }

        // GET: Product/Create - Displays the form to add a new product
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: Product/Create - Handles the form submission to add a new product
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public IActionResult Create(Product product)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                product.ShopId = GetShopId();
                _productRepo.AddProduct(product);
                return RedirectToAction(nameof(Index)); // Redirect back to the product list
            }
            return View(product); // If model state is invalid, return to the form with errors
        }

        // GET: Product/Edit/{id} - Displays the form to edit an existing product
        public IActionResult Edit(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Product? product = _productRepo.GetProductById(id, shopId);
            if (product == null)
            {
                return NotFound(); // Return 404 if product not found
            }
            return View(product);
        }

        // POST: Product/Edit - Handles the form submission to update a product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Preserve ShopId from existing product
                var shopId = GetShopId();
                product.ShopId = shopId;
                _productRepo.UpdateProduct(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/{id} - Displays a confirmation page before deleting a product
        public IActionResult Delete(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Product? product = _productRepo.GetProductById(id, shopId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/DeleteConfirmed - Handles the actual deletion of a product
        [HttpPost, ActionName("Delete")] // ActionName allows using "Delete" for both GET and POST
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            _productRepo.DeleteProduct(id, shopId);
            return RedirectToAction(nameof(Index));
        }
    }
}