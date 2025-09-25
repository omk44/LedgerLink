using LedgerLink.Models;
using System.Collections.Generic;
using System;

namespace LedgerLink.Interface
{
    public interface ICustomerRepo
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer? GetCustomerById(Guid id);
        Customer AddCustomer(Customer customer);
        Customer? UpdateCustomer(Customer customer);
        Customer? DeleteCustomer(Guid id);
    }
}