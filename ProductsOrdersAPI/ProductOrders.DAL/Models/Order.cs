using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Models
{
    public class Order : Entity
    {
        public DateTime Date { get; set; }

        public int CustomerCode { get; set; }

        public List<OrderProduct> OrderProducts { get; set; }
    }
}