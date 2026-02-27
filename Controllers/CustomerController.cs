using System; // For Guid and DateTime
using System.Collections.Generic; // For IEnumerable
using LedgerLink.Interface; // For ICustomerRepo
using LedgerLink.Models;   // For Customer model
using LedgerLink.Services; // For QrCodeService
using Microsoft.AspNetCore.Http; // Required for HttpContext.Session
using Microsoft.AspNetCore.Mvc;


namespace LedgerLink.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly QrCodeService _qrCodeService;

        public CustomerController(ICustomerRepo customerRepo, QrCodeService qrCodeService)
        {
            _customerRepo = customerRepo;
            _qrCodeService = qrCodeService;
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

        public IActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            var shopId = GetShopId();
            IEnumerable<Customer> customers = _customerRepo.GetAllCustomers(shopId);
            return View(customers);
        }

        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            // CRITICAL FIX: Remove Barcode from ModelState validation errors
            

            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }
            // Set ShopId from session
            customer.ShopId = GetShopId();
            // CRITICAL FIX: Assign a new Guid directly to Barcode

            if (ModelState.IsValid)
            {
                _customerRepo.AddCustomer(customer);
                // CRITICAL FIX: Pass the Guid.ToString() to the ShowQrCode action
                return RedirectToAction("ShowQrCode", new { id = customer.Id.ToString() });
            }
            return View(customer);
        }

        public IActionResult ShowQrCode(string id) // barcode parameter remains string as it's from URL
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Customer Id not provided.");
            }

            // CRITICAL FIX: Parse the incoming string barcode from URL to Guid
            if (!Guid.TryParse(id, out Guid parsedBarcodeGuid))
            {
                return BadRequest("Invalid customer id format.");
            }

            // CRITICAL FIX: Pass the Guid to GetCustomerByBarcode
            Customer? customer = _customerRepo.GetCustomerById(parsedBarcodeGuid, GetShopId());
            if (customer == null)
            {
                return NotFound("Customer not found for the given barcode.");
            }

            // Generate the QR code image bytes using the QrCodeService with Guid.ToString()
            byte[] qrCodeImageBytes = _qrCodeService.GenerateQrCode(customer.Id);

            ViewBag.QrCodeBase64 = Convert.ToBase64String(qrCodeImageBytes);
            ViewBag.CustomerName = customer.FullName;
            ViewBag.CustomerBarcode = customer.Id.ToString(); // Display as string
            return View(customer);
        }

        public IActionResult Edit(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Customer? customer = _customerRepo.GetCustomerById(id, shopId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Edit(Customer customer)
{
    if (!IsAdminLoggedIn())
    {
        return RedirectToAction("Login", "Account");
    }

    if (ModelState.IsValid)
    {
        var shopId = GetShopId();
        // ✅ Fetch the existing customer first
        var existingCustomer = _customerRepo.GetCustomerById(customer.Id, shopId);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        // ✅ Preserve CurrentBalance (don't reset it to 0)
        customer.CurrentBalance = existingCustomer.CurrentBalance;
        // Preserve ShopId
        customer.ShopId = existingCustomer.ShopId;

        // ✅ Now update with other changes
        _customerRepo.UpdateCustomer(customer);

        return RedirectToAction(nameof(Index));
    }

    return View(customer);
}

        public IActionResult Delete(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Customer? customer = _customerRepo.GetCustomerById(id, shopId);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            _customerRepo.DeleteCustomer(id, shopId);
            return RedirectToAction(nameof(Index));
        }
    }
}