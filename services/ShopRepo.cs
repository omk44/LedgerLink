using System;
using System.Threading.Tasks;
using LedgerLink.Data;
using LedgerLink.Interface;
using LedgerLink.Models;

namespace LedgerLink.Services
{
    public class ShopRepo : IShopRepo
    {
        private readonly AppDbContext _context;

        public ShopRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Shop?> GetByIdAsync(Guid id)
        {
            return await _context.Shops.FindAsync(id);
        }

        public async Task<Shop> CreateAsync(Shop shop)
        {
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
            return shop;
        }

        public async Task UpdateAsync(Shop shop)
        {
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }
    }
}
