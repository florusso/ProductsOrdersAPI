using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using ProductsOrders.DAL.Models.Mongo;
using ProductsOrders.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsOrders.DAL.Repository.Tests
{
    [TestClass()]
    public class ProductRepositoryTests
    {
        private Mock<IOptions<Mongosettings>> _mockOptions;

        private Mock<IMongoDatabase> _mockDB;

        private Mock<IMongoClient> _mockClient;

        public ProductRepositoryTests()
        {
            _mockOptions = new Mock<IOptions<Mongosettings>>();
            _mockDB = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [TestMethod()]
        public void ProductRepositoryUpdateTest()
        {
            var settings = new Mongosettings()
            {
                Connection = "mongodb://localhost:27017",
                DatabaseName = "ProductsOrdersDB"
            };
            var _mockOptions = new Mock<IOptions<Mongosettings>>();

            _mockOptions.Setup(s => s.Value).Returns(settings);
            _mockClient.Setup(c => c
            .GetDatabase(_mockOptions.Object.Value.DatabaseName, null))
                .Returns(_mockDB.Object);

            //Act
            var context = new MongoDBContext(_mockOptions.Object);

            var repo = new ProductRepository(context);

            var product = repo.Get("5ea9f8b21226ae3fe07e4454").GetAwaiter().GetResult();

            product.StockAmount -= 2;

            repo.Update("5ea9f8b21226ae3fe07e4454", product).GetAwaiter().GetResult();

            var productUpdated = repo.Get("5ea9f8b21226ae3fe07e4454").GetAwaiter().GetResult();
            Assert.AreEqual(product.StockAmount, productUpdated.StockAmount);
        }
    }
}