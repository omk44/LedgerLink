using LedgerLink.Models;
using System;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface ITransactionRepo
    {
        IEnumerable<Transaction> GetAllTransactions(Guid shopId);
        Transaction GetTransactionById(int id, Guid shopId);
        Transaction AddTransaction(Transaction transaction);
        Transaction UpdateTransaction(Transaction transaction);
        Transaction DeleteTransaction(int id, Guid shopId);
    }
}