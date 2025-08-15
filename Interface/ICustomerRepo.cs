// Path: LedgerLink/Interface/ICustomerRepo.cs
using LedgerLink.Models;
using System.Collections.Generic;
using System; // Required for Guid

namespace LedgerLink.Interface
{
    public interface ICustomerRepo
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer? GetCustomerById(Guid id); // Retrieves by the primary key (Guid Id)
        Customer? GetCustomerByBarcode(string barcode); // <--- NEW: Retrieves by the scannable barcode string
        Customer AddCustomer(Customer customer);
        Customer? UpdateCustomer(Customer customer);
        Customer? DeleteCustomer(Guid id);
    }
}