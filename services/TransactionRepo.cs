// Path: LedgerLink/Services/TransactionRepo.cs
using LedgerLink.Models;
using LedgerLink.Data;
using LedgerLink.Interface;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Required for Include()

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
            // CRITICAL FIX: Eager load Customer and Product for display in CustomerDetails view
            return _context.Transactions
                           .Include(t => t.Customer) // Load related Customer data
                           .Include(t => t.Product)  // Load related Product data
                           .ToList();
        }

        public Transaction? GetTransactionById(int id)
        {
            // CRITICAL FIX: Eager load Customer and Product for single transaction lookup
            return _context.Transactions
                           .Include(t => t.Customer)
                           .Include(t => t.Product)
                           .FirstOrDefault(t => t.Id == id);
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