using ProductsOrders.DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product> Get(string name, string desc);

        bool UpdateProductsAmount(List<OrderProduct> orderProducts);
    }
}