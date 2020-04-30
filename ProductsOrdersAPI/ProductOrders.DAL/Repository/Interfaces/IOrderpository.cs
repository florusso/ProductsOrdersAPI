using MongoDB.Driver;
using ProductsOrders.DAL.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository
{
    public interface IOrderepository : IBaseRepository<Order>
    {
        bool AlreadyOrdered(int customercode);

        Task<object> OpenTransactionAsync();

        void CommitTransaction(object session);

        void AbortTransaction(object session);
    }
}