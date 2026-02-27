using LedgerLink.Models;
using System.Collections.Generic;
using System;

namespace LedgerLink.Interface
{
    public interface IPaymentRepo
    {
        IEnumerable<Payment> GetAllPayments(Guid shopId);
        Payment? GetPaymentById(Guid id, Guid shopId);
        Payment AddPayment(Payment payment);
        Payment UpdatePayment(Payment payment);
        Payment DeletePayment(Guid id, Guid shopId);
    }
}