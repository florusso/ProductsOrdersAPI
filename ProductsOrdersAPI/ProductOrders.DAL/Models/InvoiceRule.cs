using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsOrders.DAL.Models
{
    public class InvoiceRule : Entity
    {
        public int CustomerCode { get; set; }

        public string Operator { get; set; }

        public double OperatorValue { get; set; }
    }
}