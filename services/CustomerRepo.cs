// Path: LedgerLink/Services/CustomerRepo.cs
using LedgerLink.Interface;
using LedgerLink.Models;
using LedgerLink.Data;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LedgerLink.Services
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _context.Customers.ToList();
        }

        public Customer? GetCustomerById(Guid id)
        {
            return _context.Customers.Find(id);
        }

        // NEW: Implementation for GetCustomerByBarcode
        public Customer? GetCustomerByBarcode(string barcode)
        {
            // Finds a customer by their unique Barcode string
            return _context.Customers.FirstOrDefault(c => c.Barcode == barcode);
        }

        public Customer AddCustomer(Customer customer)
        {
            if (customer.Id == Guid.Empty)
            {
                customer.Id = Guid.NewGuid();
            }
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer;
        }

        public Customer? UpdateCustomer(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.Id);
            if (existingCustomer != null)
            {
                _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
                _context.SaveChanges();
            }
            return existingCustomer;
        }

        public Customer? DeleteCustomer(Guid id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return customer;
        }
    }
}