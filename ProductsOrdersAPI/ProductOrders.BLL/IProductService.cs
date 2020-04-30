using ProductsOrders.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductsOrders.BLL
{
    public interface IProductService
    {
        ServiceResponse<Product> Create(Product obj);

        void Update(string id, Product obj);

        void Delete(string id);

        Task<Product> Get(string id);

        Task<IEnumerable<Product>> Get();

        bool HasTotProduct(string id, int amount);

        bool UpdateProductsAmount(List<OrderProduct> orderProducts);
    }
}