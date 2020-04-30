using ProductsOrders.DAL.Models;
using System.Collections.Generic;

namespace ProductsOrders.BLL
{
    public interface IOrderService
    {
        ServiceResponse<Order> Create(Order order);
        IEnumerable<Order> Get();
    }
}