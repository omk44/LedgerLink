using LedgerLink.Models;
using System.Collections.Generic;
using System;

namespace LedgerLink.Interface
{
    public interface ICustomerRepo
    {
        IEnumerable<Customer> GetAllCustomers(Guid shopId);
        Customer? GetCustomerById(Guid id, Guid shopId);
        Customer AddCustomer(Customer customer);
        Customer? UpdateCustomer(Customer customer);
        Customer? DeleteCustomer(Guid id, Guid shopId);
    }
}