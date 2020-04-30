using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Models.Mongo;
using ProductsOrders.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsOrders.DAL.Repository.Tests
{
    [TestClass()]
    public class InvoiceRuleRepositoryTests
    {
        private Mock<IOptions<Mongosettings>> _mockOptions;

        private Mock<IMongoDatabase> _mockDB;

        private Mock<IMongoClient> _mockClient;

        public InvoiceRuleRepositoryTests()
        {
            _mockOptions = new Mock<IOptions<Mongosettings>>();
            _mockDB = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [TestMethod()]
        public void CreateInvoiceRuleRepositoryTest()
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

            var repo = new InvoiceRuleRepository(context);
            var Inovoicerule1 = new InvoiceRule
            {
                Id = null,
                CustomerCode = 1,
                Operator = "%",
                OperatorValue = 0.02
            };

            var Inovoicerule2 = new InvoiceRule
            {
                Id = null,
                CustomerCode = 2,
                Operator = "+",
                OperatorValue = 1
            };
            var ret = repo.Create(Inovoicerule1).GetAwaiter().GetResult();
            Assert.IsNotNull(ret);
            ret = repo.Create(Inovoicerule2).GetAwaiter().GetResult();
            Assert.IsNotNull(ret);
        }

        [TestMethod()]
        public void GetInvoiceRuleRepositoryTest()
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

            var repo = new InvoiceRuleRepository(context);

            var ret = repo.Get().GetAwaiter().GetResult();
            Assert.IsNotNull(ret);
        }
    }
}