using MongoDB.Driver;
using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Models.Mongo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository
{
    public class ProductRepository : MongoBaseRepository<Product>, IProductRepository
    {
        private IMongoDBContext _context;

        public ProductRepository(IMongoDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> Get(string name, string desc)
        {
            var prod = await _dbCollection.FindAsync(p => p.Name == name && p.Desc == desc);
            return await prod.FirstOrDefaultAsync();
        }

        public bool UpdateProductsAmount(List<OrderProduct> orderProducts)
        {
            var client = _context.GetMongoClient();

            using (var session = client.StartSessionAsync().GetAwaiter().GetResult())
            {
                session.StartTransaction();
                foreach (var item in orderProducts)
                {
                    var prod = Get(item.Id).GetAwaiter().GetResult();
                    prod.StockAmount -= item.Amount;
                    if (prod.StockAmount < 0)
                    {
                        session.AbortTransaction();
                        throw new Exception("errore quantità in stock non presente");
                    }
                    try
                    {
                        Update(prod.Id, prod).GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {
                        session.AbortTransaction();
                        throw;
                    }
                }
                session.CommitTransaction();
            }

            return true;
        }
    }
}