// Path: LedgerLink/Services/TransactionRepo.cs
using LedgerLink.Models;
using LedgerLink.Data; // Required to inject AppDbContext
using LedgerLink.Interface;
using System.Collections.Generic;
using System.Linq;

namespace LedgerLink.Services
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly AppDbContext _context;

        public TransactionRepo(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> GetAllTransactions()
        {
            // Include navigation properties if you want to load related Customer and Product data
            // For example: return _context.Transactions.Include(t => t.Customer).Include(t => t.Product).ToList();
            return _context.Transactions.ToList();
        }

        public Transaction? GetTransactionById(int id)
        {
            // Include navigation properties if you want to load related Customer and Product data
            // For example: return _context.Transactions.Include(t => t.Customer).Include(t => t.Product).FirstOrDefault(t => t.Id == id);
            return _context.Transactions.Find(id);
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
            return transaction;
        }

        public Transaction? UpdateTransaction(Transaction transaction)
        {
            var existingTransaction = _context.Transactions.Find(transaction.Id);
            if (existingTransaction != null)
            {
                _context.Entry(existingTransaction).CurrentValues.SetValues(transaction);
                _context.SaveChanges();
            }
            return existingTransaction;
        }

        public Transaction? DeleteTransaction(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
            return transaction;
        }
    }
}