using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LedgerLink.Interface;
using LedgerLink.Models;
using LedgerLink.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using X.PagedList.Extensions;

namespace LedgerLink.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IProductRepo _productRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IPaymentRepo _paymentRepo;
        private readonly IEmailService _emailService;
        private readonly IFestivalRepo _festivalRepo;
        private readonly IDiscountRuleRepo _discountRuleRepo;
        private readonly IShopRepo _shopRepo;
        private readonly IAdminRepo _adminRepo;

        public TransactionController(
            ICustomerRepo customerRepo,
            IProductRepo productRepo,
            ITransactionRepo transactionRepo,
            IPaymentRepo paymentRepo,
            IEmailService emailService,
            IFestivalRepo festivalRepo,
            IDiscountRuleRepo discountRuleRepo,
            IShopRepo shopRepo,
            IAdminRepo adminRepo)
        {
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _transactionRepo = transactionRepo;
            _paymentRepo = paymentRepo;
            _emailService = emailService;
            _festivalRepo = festivalRepo;
            _discountRuleRepo = discountRuleRepo;
            _shopRepo = shopRepo;
            _adminRepo = adminRepo;
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

        public IActionResult Scan()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ProcessScan([FromBody] string customerIdString)
        {
            if (!IsAdminLoggedIn())
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(customerIdString))
            {
                return BadRequest("Customer ID cannot be empty.");
            }

            if (!Guid.TryParse(customerIdString.Trim(), out Guid scannedCustomerId))
            {
                return BadRequest("Invalid customer ID format received.");
            }

            var shopId = GetShopId();
            Customer? customer = _customerRepo.GetCustomerById(scannedCustomerId, shopId);

            if (customer == null)
            {
                return NotFound("Customer not found for the scanned ID.");
            }

            return Ok(new { customerId = customer.Id });
        }


public IActionResult CustomerDetails(Guid id, int transactionPage = 1, int paymentPage = 1)
{
    if (!IsAdminLoggedIn())
    {
        return RedirectToAction("Login", "Account");
    }

    var shopId = GetShopId();
    var customer = _customerRepo.GetCustomerById(id, shopId);
    if (customer == null)
    {
        return NotFound("Customer not found.");
    }

    var products = _productRepo.GetAllProducts(shopId);

    var activeFestivals = _festivalRepo.GetAllFestivals(shopId)
        .Where(f => f.IsActive && f.StartDate.Date <= DateTime.UtcNow.Date && f.EndDate.Date >= DateTime.UtcNow.Date)
        .ToList();

    var transactions = _transactionRepo.GetAllTransactions(shopId)
        .Where(t => t.CustomerId == id)
        .OrderByDescending(t => t.PurchaseDate)
        .ToPagedList(transactionPage, 10);

    var payments = _paymentRepo.GetAllPayments(shopId)
        .Where(p => p.CustomerId == id)
        .OrderByDescending(p => p.PaymentDate)
        .ToPagedList(paymentPage, 10);

    var activeRules = _discountRuleRepo.GetAllDiscountRules(shopId)
        .Where(r => activeFestivals.Select(f => f.Id).Contains(r.FestivalId))
        .ToList();

    var viewModel = new CustomerDetailsViewModel
    {
        Customer = customer,
        Products = products,
        Transactions = transactions,
        Payments = payments,
        ActiveFestivals = activeFestivals,
        ActiveDiscountRules = activeRules
    };

    return View(viewModel);
}





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(Guid customerId, int productId, int quantity, bool isCreditTransaction, string? notes, string? paymentMode, int? applyDiscountFestivalId)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Customer? customer = _customerRepo.GetCustomerById(customerId, shopId);
            Product? product = _productRepo.GetProductById(productId, shopId);

            if (customer == null || product == null)
            {
                TempData["ErrorMessage"] = "Customer or Product not found.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            decimal totalAmount = product.Price * quantity;
            decimal discountPercentage = 0.00m;
            decimal discountAmount = 0.00m;
            decimal finalAmount = totalAmount;
            int? festivalId = null;

            if (applyDiscountFestivalId.HasValue)
            {
                Festival? activeFestival = _festivalRepo.GetFestivalById(applyDiscountFestivalId.Value, shopId);

                if (activeFestival != null)
                {
                    DiscountRule? matchingRule = _discountRuleRepo.GetAllDiscountRules(shopId)
                                                                  .Where(r => r.FestivalId == activeFestival.Id)
                                                                  .FirstOrDefault(r =>
                                                                      customer.CurrentBalance >= r.MinCustomerCreditBalance &&
                                                                      customer.CurrentBalance <= r.MaxCustomerCreditBalance &&
                                                                      totalAmount >= r.MinPurchaseAmount);

                    if (matchingRule != null)
                    {
                        discountPercentage = matchingRule.DiscountPercentage;
                        discountAmount = totalAmount * (discountPercentage / 100);
                        finalAmount = totalAmount - discountAmount;
                        festivalId = activeFestival.Id;
                    }
                }
            }

            var newTransaction = new Transaction
            {
                Id = 0,
                ShopId = shopId,
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price,
                TotalAmount = totalAmount,
                FinalAmount = finalAmount,
                IsCreditTransaction = isCreditTransaction,
                PurchaseDate = DateTime.UtcNow,
                Notes = notes,
                QuantityUnit = product.QuantityUnit,
                FestivalId = festivalId,
                DiscountPercentage = discountPercentage,
                DiscountAmount = discountAmount
            };

            _transactionRepo.AddTransaction(newTransaction);

            if (isCreditTransaction)
            {
                customer.CurrentBalance += finalAmount;
                _customerRepo.UpdateCustomer(customer);
                TempData["SuccessMessage"] = "Item added successfully (on credit)!";
            }
            else
            {
                if (string.IsNullOrEmpty(paymentMode))
                {
                    TempData["ErrorMessage"] = "Payment mode is required for paid transactions.";
                    return RedirectToAction("CustomerDetails", new { id = customerId });
                }

                var newPayment = new Payment
                {
                    Id = Guid.NewGuid(),
                    ShopId = shopId,
                    CustomerId = customerId,
                    AmountPaid = finalAmount,
                    PaymentMode = paymentMode,
                    PaymentDate = DateTime.UtcNow
                };
                _paymentRepo.AddPayment(newPayment);
                TempData["SuccessMessage"] = "Item added successfully (paid)!";
            }

            return RedirectToAction("ShowReceipt", new { transactionId = newTransaction.Id, isTransaction = true });
        }

        public IActionResult AddPayment(Guid customerId, decimal amountPaid, string paymentMode)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            Customer? customer = _customerRepo.GetCustomerById(customerId, shopId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found for payment.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            if (amountPaid <= 0)
            {
                TempData["ErrorMessage"] = "Amount paid must be greater than zero.";
                return RedirectToAction("CustomerDetails", new { id = customerId });
            }

            var newPayment = new Payment
            {
                Id = Guid.NewGuid(),
                ShopId = shopId,
                CustomerId = customerId,
                AmountPaid = amountPaid,
                PaymentMode = paymentMode,
                PaymentDate = DateTime.UtcNow
            };

            _paymentRepo.AddPayment(newPayment);

            customer.CurrentBalance -= amountPaid;
            // if (customer.CurrentBalance < 0)
            // {
            //     customer.CurrentBalance = 0;
            // }

            _customerRepo.UpdateCustomer(customer);

            TempData["SuccessMessage"] = "Payment recorded successfully!";
            return RedirectToAction("ShowReceipt", new { paymentId = newPayment.Id, isTransaction = false });
        }

        public async Task<IActionResult> ShowReceipt(int? transactionId, Guid? paymentId, bool isTransaction)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var shopId = GetShopId();
            var adminIdString = HttpContext.Session.GetString("UserId");
            
            // Get shop and admin details from database
            var shop = await _shopRepo.GetByIdAsync(shopId);
            var admin = adminIdString != null && Guid.TryParse(adminIdString, out var adminId) 
                ? await _adminRepo.GetByIdAsync(adminId) 
                : null;

            ReceiptViewModel viewModel = new ReceiptViewModel
            {
                ShopName = shop?.ShopName ?? "Shop",
                AdminName = admin?.FullName ?? "Admin",
                AdminEmail = admin?.Email ?? ""
            };
            Customer? customer = null;

            if (isTransaction && transactionId.HasValue)
            {
                Transaction? transaction = _transactionRepo.GetAllTransactions(shopId)
                                                          .FirstOrDefault(t => t.Id == transactionId.Value);
                if (transaction == null)
                {
                    return NotFound("Transaction receipt not found.");
                }

                customer = _customerRepo.GetCustomerById(transaction.CustomerId, shopId);

                viewModel.ReceiptType = "Sale";
                viewModel.Transaction = transaction;
                viewModel.TransactionItems = new List<Transaction> { transaction };
                viewModel.AmountPaidInThisReceipt = transaction.TotalAmount;
            }
            else if (!isTransaction && paymentId.HasValue)
            {
                Payment? payment = _paymentRepo.GetAllPayments(shopId)
                                              .FirstOrDefault(p => p.Id == paymentId.Value);
                if (payment == null)
                {
                    return NotFound("Payment receipt not found.");
                }

                customer = _customerRepo.GetCustomerById(payment.CustomerId, shopId);

                viewModel.ReceiptType = "Payment";
                viewModel.Payment = payment;
                viewModel.AmountPaidInThisReceipt = payment.AmountPaid;
            }
            else
            {
                return BadRequest("Invalid receipt request.");
            }

            if (customer == null)
            {
                return NotFound("Customer associated with receipt not found.");
            }

            viewModel.Customer = customer;
            viewModel.CustomerNewBalance = _customerRepo.GetCustomerById(customer.Id, shopId)?.CurrentBalance ?? 0.00m;

            return View(viewModel);
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SendReminder(Guid customerId)
{
    if (!IsAdminLoggedIn())
    {
        return Unauthorized();
    }

    var shopId = GetShopId();
    Customer? customer = _customerRepo.GetCustomerById(customerId, shopId);
    if (customer == null)
    {
        TempData["ErrorMessage"] = "Customer not found.";
        return RedirectToAction("Index", "Customer");
    }

    if (string.IsNullOrEmpty(customer.Email))
    {
        TempData["ErrorMessage"] = "Cannot send reminder: This customer does not have an email address on file.";
        return RedirectToAction("CustomerDetails", new { id = customerId });
    }

    // Get admin and shop details from database
    var adminIdString = HttpContext.Session.GetString("UserId");
    var shop = await _shopRepo.GetByIdAsync(shopId);
    var admin = adminIdString != null && Guid.TryParse(adminIdString, out var adminId) 
        ? await _adminRepo.GetByIdAsync(adminId) 
        : null;

    var shopName = shop?.ShopName ?? "Our Shop";
    var adminName = admin?.FullName ?? "Admin";
    var adminEmail = admin?.Email ?? "";

    var indiaCulture = new CultureInfo("en-IN");
    string formattedBalance = customer.CurrentBalance.ToString("C", indiaCulture);

    string subject = $"Payment Reminder from {shopName}";
    string messageBody = $"Dear {customer.FullName},\n\n" +
        $"This is a friendly reminder that your outstanding balance at {shopName} is {formattedBalance}.\n\n" +
        $"Please settle your dues at your earliest convenience.\n\n" +
        $"Thank you,\n" +
        $"{adminName}\n" +
        $"{shopName}\n" +
        $"Contact: {adminEmail}";

    bool emailSent = await _emailService.SendEmailAsync(customer.Email, subject, messageBody);

    if (emailSent)
    {
        TempData["SuccessMessage"] = "Payment reminder email sent successfully!";
    }
    else
    {
        TempData["ErrorMessage"] = "Failed to send payment reminder. Please check the system logs for details.";
    }

    return RedirectToAction("CustomerDetails", new { id = customerId });
}

        // Test email endpoint - remove in production
        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            if (!IsAdminLoggedIn())
            {
                return Unauthorized();
            }

            try
            {
                var adminEmail = HttpContext.Session.GetString("AdminEmail") ?? "test@example.com";
                bool emailSent = await _emailService.SendEmailAsync(
                    adminEmail, 
                    "LedgerLink Email Test", 
                    "This is a test email from LedgerLink. If you received this, your email configuration is working correctly!");

                if (emailSent)
                {
                    return Ok(new { success = true, message = $"Test email sent successfully to {adminEmail}. Check your inbox!" });
                }
                else
                {
                    return Ok(new { success = false, message = "Failed to send test email. Check application logs for details." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = $"Error: {ex.Message}" });
            }
        }


        [HttpPost]
        public IActionResult CalculateDiscount([FromBody] CalculateDiscountRequestModel request)
        {
            var shopId = GetShopId();
            decimal originalAmount = request.quantity * request.unitPrice;
            decimal discountPercentage = 0.00m;
            decimal discountAmount = 0.00m;
            decimal finalAmount = originalAmount;
            string? message = null;

            if (request.applyDiscountFestivalId.HasValue)
            {
                Festival? selectedFestival = _festivalRepo.GetFestivalById(request.applyDiscountFestivalId.Value, shopId);

                if (selectedFestival != null && selectedFestival.IsActive &&
                    selectedFestival.StartDate.Date <= DateTime.UtcNow.Date &&
                    selectedFestival.EndDate.Date >= DateTime.UtcNow.Date)
                {
                    Customer? customer = _customerRepo.GetCustomerById(request.customerId, shopId);
                    if (customer != null)
                    {
                        DiscountRule? matchingRule = _discountRuleRepo.GetAllDiscountRules(shopId)
                            .Where(r => r.FestivalId == selectedFestival.Id)
                            .FirstOrDefault(r =>
                                customer.CurrentBalance >= r.MinCustomerCreditBalance &&
                                customer.CurrentBalance <= r.MaxCustomerCreditBalance &&
                                originalAmount >= r.MinPurchaseAmount);

                        if (matchingRule != null)
                        {
                            discountPercentage = matchingRule.DiscountPercentage;
                            discountAmount = originalAmount * (discountPercentage / 100);
                            finalAmount = originalAmount - discountAmount;
                            message = $"Festival offer applied! {discountPercentage}% discount on this item.";
                        }
                        else
                        {
                            message = "No matching discount rule found for this customer.";
                        }
                    }
                    else
                    {
                        message = "Customer not found for discount calculation.";
                    }
                }
                else
                {
                    message = "Selected festival is not active.";
                }
            }
            else
            {
                message = "No festival selected.";
            }

            return Ok(new { discountPercentage, discountAmount, finalAmount, message });
        }
    }
}
