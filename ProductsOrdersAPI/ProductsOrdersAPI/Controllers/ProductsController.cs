using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsOrders.BLL;
using ProductsOrders.DAL.Models;

namespace ProductsOrdersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        private IProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, IProductService productService)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> Get()

        {
            _logger.LogInformation(" GET: api/Products");
            return _productService.Get().GetAwaiter().GetResult();
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public Product Get(string id)
        {
            _logger.LogInformation($"GET: api/Products/{id}");
            return _productService.Get(id).GetAwaiter().GetResult();
        }

        // POST: api/Products
        [HttpPost]
        public Product Post([FromBody] Product product)
        {
            var ret = new ServiceResponse<Product>();
            try
            {
                product.Id = null;
                ret = _productService.Create(product);
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