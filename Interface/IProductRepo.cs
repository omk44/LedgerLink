using LedgerLink.Models;
using System;
using System.Collections.Generic;

namespace LedgerLink.Interface
{
    public interface IProductRepo
    {
        IEnumerable<Product> GetAllProducts(Guid shopId);
        Product GetProductById(int id, Guid shopId);
        Product AddProduct(Product product);
        Product UpdateProduct(Product product);
        Product DeleteProduct(int id, Guid shopId);
    }
}