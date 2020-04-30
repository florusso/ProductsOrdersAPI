using MongoDB.Driver;

namespace ProductsOrders.DAL.Models.Mongo
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);

        IMongoClient GetMongoClient();
    }
}