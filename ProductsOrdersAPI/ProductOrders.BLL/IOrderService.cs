using ProductsOrders.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsOrders.BLL
{
    public interface IOrderService
    {
        Task<ServiceResponse<Order>> CreateAsync(Order order);

        Task<IEnumerable<Order>> GetAsync();
    }
}