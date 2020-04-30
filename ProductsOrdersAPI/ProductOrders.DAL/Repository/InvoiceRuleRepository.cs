using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Models.Mongo;
using ProductsOrders.DAL.Repository.Interfaces;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ProductsOrders.DAL.Repository
{
    public class InvoiceRuleRepository : MongoBaseRepository<InvoiceRule>, IInvoiceRuleRepository
    {
        public InvoiceRuleRepository(IMongoDBContext context) : base(context)
        {
        }

        public async Task<InvoiceRule> Get(int customerCode)
        {
            var prod = await _dbCollection.FindAsync(p => p.CustomerCode == customerCode);
            return await prod.FirstOrDefaultAsync();
        }
    }
}