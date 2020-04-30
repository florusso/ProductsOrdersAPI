using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Models
{
    public class Product : Entity
    {
        //-	Prodotti(id, nome, descrizione, prezzo unitario, quantità a stock)

        public string Name { get; set; }
        public string Desc { get; set; }

        public double UnitPrice { get; set; }

        public int StockAmount { get; set; }
    }
}