using ProductsOrders.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductsOrders.BLL
{
    public interface IProductService
    {
        Task<ServiceResponse<Product>> CreateAsync(Product obj);

        Task UpdateAsync(string id, Product obj);

        void Delete(string id);

        Task<Product> Get(string id);

        Task<IEnumerable<Product>> Get();

        // async Task<bool> HasTotProductAsync
        Task<bool> UpdateProductsAmountAsync(List<OrderProduct> orderProducts);

        Task<bool> HasTotProductAsync(string id, int amount);
    }
}