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

        public IEnumerable<Customer> GetAllCustomers(Guid shopId)
        {
            return _context.Customers.Where(c => c.ShopId == shopId).ToList();
        }

        public Customer? GetCustomerById(Guid id, Guid shopId)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id && c.ShopId == shopId);
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

        public Customer? DeleteCustomer(Guid id, Guid shopId)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id && c.ShopId == shopId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return customer;
        }
    }
}