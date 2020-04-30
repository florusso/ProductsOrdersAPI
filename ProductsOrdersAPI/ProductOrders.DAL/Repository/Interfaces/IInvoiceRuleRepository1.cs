using ProductsOrders.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository.Interfaces
{
    public interface IInvoiceRuleRepository : IBaseRepository<InvoiceRule>
    {
        Task<InvoiceRule> Get(int customerCode);
    }
}