using System.Threading.Tasks;

namespace ProductsOrders.BLL
{
    public interface IInvoiceRuleService
    {
        Task<double> ApplyAsync(int CustomerCode, double summation);
    }
}