using LedgerLink.Models;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface ITransactionRepo
    {
        IEnumerable<Transaction> GetAllTransactions();
        Transaction GetTransactionById(int id);
        Transaction AddTransaction(Transaction transaction);
        Transaction UpdateTransaction(Transaction transaction);
        Transaction DeleteTransaction(int id);
    }
}