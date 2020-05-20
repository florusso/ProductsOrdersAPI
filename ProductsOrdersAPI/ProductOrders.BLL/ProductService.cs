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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private IMemoryCache _cache;

        private const string ProductCacheKey = "Product-cache-key";

        private const int MaxUnitPrice = 1000;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<ServiceResponse<Product>> CreateAsync(Product obj)
        {
            var ret = new ServiceResponse<Product>();
            try
            {
                var exist = await _productRepository.Get(obj.Name, obj.Desc) != null;

                if (exist)
                {
                    ret.IsSuccess = false;
                    ret.ResponseMessage = "Product already Exist";
                    return ret;
                }

                var isTooExpensive = obj.UnitPrice > MaxUnitPrice;

                if (isTooExpensive)
                {
                    ret.IsSuccess = false;
                    ret.ResponseMessage = $"Product Max unit price is:{MaxUnitPrice} euro";
                    return ret;
                }

                await _productRepository.Create(obj);
                ret.IsSuccess = true;
                ret.ResponseMessage = "Ok";
                ret.ResponseObject = obj;

                _cache.TryGetValue(ProductCacheKey, out List<Product> _products);
                _products.Add(obj);
                _cache.Set(ProductCacheKey, _products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error");
                throw;
            }
            return ret;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Get(string id)
        {
            var p = new Product();

            var _products = _cache.GetOrCreateAsync(ProductCacheKey, entry =>
               {
                   return _productRepository.Get();
               });

            p = _products.Result.FirstOrDefault(p => p.Id == id);

            return Task.FromResult(p);
        }

        public Task<IEnumerable<Product>> Get()
        {
            return _cache.GetOrCreateAsync(ProductCacheKey, entry =>
          {
              return _productRepository.Get();
          });
        }

        public async Task UpdateAsync(string id, Product obj)
        {
            try
            {
                await _productRepository.Update(id, obj);

                if (_cache.TryGetValue(ProductCacheKey, out List<Product> _products))
                {
                    var toUpdate = _products.FirstOrDefault(p => p.Id.ToString() == id);

                    toUpdate.Name = obj.Name;
                    toUpdate.StockAmount = obj.StockAmount;
                    toUpdate.UnitPrice = obj.UnitPrice;

                    _cache.Set(ProductCacheKey, _products);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error");
                throw;
            }
        }

        public async Task<bool> HasTotProductAsync(string id, int amount)
        {
            var products = await Get();

            var prod = products.SingleOrDefault(s => s.Id == id);

            if (prod == null) return false;

            return prod.StockAmount >= amount;
        }

        public async Task<bool> UpdateProductsAmountAsync(List<OrderProduct> orderProducts)
        {
            if (_productRepository.UpdateProductsAmount(orderProducts))
            {
                var _products = await _cache.GetOrCreateAsync(ProductCacheKey, entry =>
                {
                    return _productRepository.Get();
                });

                foreach (var item in orderProducts)
                {
                    var toUpdate = _products.FirstOrDefault(p => p.Id.ToString() == item.Id);
                    if (toUpdate != null)
                    {
                        toUpdate.StockAmount -= item.Amount;

                        _cache.Set(ProductCacheKey, _products);
                    }
                }
                return true;
            }

            return false;
        }
    }
}