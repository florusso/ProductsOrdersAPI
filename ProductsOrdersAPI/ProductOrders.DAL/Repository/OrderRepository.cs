using MongoDB.Bson;
using MongoDB.Driver;
using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Models.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository
{
    public class OrderRepository : MongoBaseRepository<Order>, IOrderepository
    {
        private IMongoDBContext _context;
        private IClientSessionHandle _session;

        public OrderRepository(IMongoDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<object> OpenTransactionAsync()
        {
            var client = _context.GetMongoClient();

            _session = await client.StartSessionAsync();
            _session.StartTransaction();
            return _session;
        }

        public override async Task<Order> Create(Order obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(typeof(Order).Name + " object is null");
            }

            try
            {
                _dbCollection = _mongoContext.GetCollection<Order>(typeof(Order).Name);

                await _dbCollection.InsertOneAsync(obj);
            }
            catch (Exception)
            {
                AbortTransaction(_session);
                throw;
            }

            return obj;
        }

        public async void CommitTransaction(object session)
        {
            if (session != null)
            {
                IClientSession clientSession = session as IClientSession;
                await clientSession.CommitTransactionAsync();
            }
        }

        public async void AbortTransaction(object session)
        {
            if (session != null)
            {
                IClientSession clientSession = session as IClientSession;
                await clientSession.AbortTransactionAsync();
            }
        }

        public bool AlreadyOrdered(int customercode)
        {
            var min = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var max = DateTime.Now;
            var prod = _dbCollection.Find(f => f.CustomerCode == customercode & f.Date > min & f.Date < max).ToList();
            return prod.Count > 0;
        }
    }
}