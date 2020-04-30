namespace ProductsOrders.DAL.Models
{
    public class OrderProduct : Entity
    {
        public double Price { get; set; }
        public int Amount { get; set; }
    }
}