// Path: LedgerLink/Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using LedgerLink.Interface; // For IProductRepo
using LedgerLink.Models;   // For Product model
using Microsoft.AspNetCore.Http; // For HttpContext.Session (manual auth check)
using System.Collections.Generic; // For IEnumerable
using System.Linq; // For LINQ methods

namespace LedgerLink.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo;

        public ProductController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        // --- Manual Session Check for Protection ---
        // This method checks if the admin is logged in.
        // REMINDER: This is an insecure placeholder for learning.
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("IsAdminLoggedIn") == "true";
        }

        // GET: Product/Index - Displays a list of all products
        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }
            IEnumerable<Product> products = _productRepo.GetAllProducts();
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

            Product? product = _productRepo.GetProductById(id);
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

            Product? product = _productRepo.GetProductById(id);
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

            _productRepo.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }
    }
}