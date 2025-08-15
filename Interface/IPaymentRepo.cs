using LedgerLink.Models;

namespace LedgerLink.Interface
{
    public interface IPaymentRepo
    {
        IEnumerable<Payment> GetAllPayments();
        Payment GetPaymentById(Guid id);
        Payment AddPayment(Payment payment);
        Payment UpdatePayment(Payment payment);
        Payment DeletePayment(Guid id);
    }
}