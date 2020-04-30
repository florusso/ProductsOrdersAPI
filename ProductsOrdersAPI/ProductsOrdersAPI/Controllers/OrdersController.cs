using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsOrders.BLL;
using ProductsOrders.DAL.Models;
using ProductsOrders.WebAPI;
using ProductsOrders.WebAPI.Helpers;

namespace ProductsOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private IOrderService _orderService;

        public OrdersController(ILogger<OrdersController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        //// GET: api/Orders
        [HttpGet]
        public PagedList<Order> GetOrders([FromQuery] OrdersParameters ordersParameters)
        {
            return PagedList<Order>.ToPagedList(_orderService.Get().AsQueryable().OrderBy(on => on.CustomerCode),
                ordersParameters.PageNumber,
                ordersParameters.PageSize);
        }

        // POST: api/Orders
        [HttpPost]
        public Order Post([FromBody] Order order)
        {
            var ret = new ServiceResponse<Order>();

            try
            {
                order.Id = null;
                ret = _orderService.Create(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error");
                ret.IsSuccess = false;
                ret.ResponseMessage = "Internal Error";
            }

            if (ret.IsSuccess)
                return ret.ResponseObject;
            else
            {
                throw new Exception(ret.ResponseMessage);
            }
        }
    }
}