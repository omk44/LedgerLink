using LedgerLink.Models;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(int id);
        Product AddProduct(Product product);
        Product UpdateProduct(Product product);
        Product DeleteProduct(int id);
    }
}