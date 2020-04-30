﻿using Microsoft.Extensions.Caching.Memory;
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

        public ServiceResponse<Product> Create(Product obj)
        {
            var ret = new ServiceResponse<Product>();
            try
            {
                var exist = _productRepository.Get(obj.Name, obj.Desc).GetAwaiter().GetResult() != null;

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

                _productRepository.Create(obj).GetAwaiter().GetResult();
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
               }).GetAwaiter().GetResult();

            p = _products.FirstOrDefault(p => p.Id == id);

            if (p is null)
                return _productRepository.Get(id);
            else return Task.FromResult(p);
        }

        public Task<IEnumerable<Product>> Get()
        {
            return _cache.GetOrCreateAsync(ProductCacheKey, entry =>
          {
              return Task.FromResult(_productRepository.Get().GetAwaiter().GetResult());
          });
        }

        public void Update(string id, Product obj)
        {
            try
            {
                _productRepository.Update(id, obj).GetAwaiter().GetResult();

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

        public bool HasTotProduct(string id, int amount)
        {
            var products = Get().GetAwaiter().GetResult();

            var prod = products.SingleOrDefault(s => s.Id == id);

            if (prod == null) return false;

            return prod.StockAmount >= amount;
        }

        public bool UpdateProductsAmount(List<OrderProduct> orderProducts)
        {
            if (_productRepository.UpdateProductsAmount(orderProducts))
            {
                var _products = _cache.GetOrCreateAsync(ProductCacheKey, entry =>
                {
                    return _productRepository.Get();
                }).GetAwaiter().GetResult();

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