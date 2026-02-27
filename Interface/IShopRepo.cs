using System;
using System.Threading.Tasks;
using LedgerLink.Models;

namespace LedgerLink.Interface
{
    public interface IShopRepo
    {
        Task<Shop?> GetByIdAsync(Guid id);
        Task<Shop> CreateAsync(Shop shop);
        Task UpdateAsync(Shop shop);
    }
}
