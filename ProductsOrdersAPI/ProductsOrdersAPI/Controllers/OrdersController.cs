﻿using System;
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
        public async Task<PagedList<Order>> GetOrdersAsync([FromQuery] OrdersParameters ordersParameters)
        {
            var orders = await _orderService.GetAsync();
            return PagedList<Order>.ToPagedList(
                orders.AsQueryable().OrderBy(on => on.CustomerCode),
                ordersParameters.PageNumber,
                ordersParameters.PageSize
                );
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<Order> PostAsync([FromBody] Order order)
        {
            var ret = new ServiceResponse<Order>();

            try
            {
                order.Id = null;
                ret = await _orderService.CreateAsync(order);
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