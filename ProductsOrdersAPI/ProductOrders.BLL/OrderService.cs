using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsOrders.BLL
{
    public class OrderService : IOrderService
    {
        private readonly IOrderepository _orderpository;
        private readonly IProductService _productService;
        private readonly IInvoiceRuleService _invoiceRuleService;

        private const string ProductCacheKey = "Product-cache-key";

        private const double MinOrderAmount = 100.00;

        public OrderService(IOrderepository orderpository, IProductService productService, IInvoiceRuleService invoiceRuleService)
        {
            _orderpository = orderpository;
            _productService = productService;
            _invoiceRuleService = invoiceRuleService;
        }

        public ServiceResponse<Order> Create(Order order)
        {
            var ret = new ServiceResponse<Order>();
            try
            {
                //deve essere stato specificato almeno un prodotto
                if (order.OrderProducts.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ResponseMessage = "Nessun prodotto specificato nell'ordine";
                    return ret;
                }
                //	il cliente non deve aver eseguito un ordine nello stesso giorno
                if (_orderpository.AlreadyOrdered(order.CustomerCode))
                {
                    ret.IsSuccess = false;
                    ret.ResponseMessage = "Il cliente ha già effettuato un ordine oggi.";
                    return ret;
                }
                // la quantità dei prodotti ordinati deve essere presente a stock
                foreach (var item in order.OrderProducts)
                {
                    if (!_productService.HasTotProduct(item.Id, item.Amount))
                    {
                        ret.IsSuccess = false;
                        ret.ResponseMessage = $"la quantità richiesta di {item.Id} non sufficente o prodotto non presente.";
                        return ret;
                    }
                }

                //il totale deve essere almeno 100 euro
                if (order.OrderProducts.Sum(s => s.Price * s.Amount) < MinOrderAmount)
                {
                    ret.IsSuccess = false;
                    ret.ResponseMessage = $"Il totale dell'ordine è inferiore a euro: {MinOrderAmount}";
                    return ret;
                }
                //  var session = _orderpository.OpenTransactionAsync().GetAwaiter().GetResult();
                /* NB: non essendo riuscito a gestire la doppia transazione in MongoDB
                 * ho deciso di effetturare almeno la seconda operazione sotto transazione
                 * e quindi effettuarla come prima  operazione
                 * se va bene allora inserisco l'ordine
                 */
                if (_productService.UpdateProductsAmount(order.OrderProducts))
                {
                    _orderpository.Create(order).GetAwaiter().GetResult();
                    //  _orderpository.CommitTransaction(session);
                }
                else
                {
                    // _orderpository.AbortTransaction(session);
                }

                double Total = CalcTotalOrder(order);
                ret.IsSuccess = true;
                ret.ResponseMessage = "Ok";
                ret.ResponseObject = order;
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }

        private double CalcTotalOrder(Order order)
        {
            double total = 0;
            double summation = 0;

            summation = order.OrderProducts.Sum(s => s.Price * s.Amount);
            try
            {
                total = _invoiceRuleService.Apply(order.CustomerCode, summation);
            }
            catch (Exception)
            {
                throw;
            }

            return total;
        }

        public IEnumerable<Order> Get()
        {
            return _orderpository.Get().GetAwaiter().GetResult();
        }
    }
}