using MongoDB.Driver;

namespace ProductsOrders.DAL.Models.Mongo
{
    public class Mongosettings
    {
        public string Connection { get; set; }

        public string DatabaseName { get; set; }
    }
}