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
        public async Task<IEnumerable<Product>> GetAsync()

        {
            _logger.LogInformation(" GET: api/Products");
            return await _productService.Get();
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<Product> GetAsync(string id)
        {
            _logger.LogInformation($"GET: api/Products/{id}");
            return await _productService.Get(id);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<Product> PostAsync([FromBody] Product product)
        {
            var ret = new ServiceResponse<Product>();
            try
            {
                product.Id = null;
                ret = await _productService.CreateAsync(product);
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