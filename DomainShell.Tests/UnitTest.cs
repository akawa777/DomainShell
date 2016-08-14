using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Apps.Cart;

namespace DomainShell.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            Session.Config(DataStoreProvider.CreateConnection, DataStoreProvider.CreateDataAdapter);
        }

        [TestMethod]
        public void Test01()
        {
            CartApp app = new CartApp();

            app.Add("1", new CartItem { ProductId = "1", Number = 1 });
            app.Add("1", new CartItem { ProductId = "2", Number = 2 });

            CartItem[] items = app.Get("1");

            Assert.AreEqual(2, items.Length);

            Payment payment = new Payment
            {
                CartId = items[0].CartId,
                CreditCardNo = "xxx",
                CreditCardHolder = "xxx",
                CreditCardExpirationDate = "xxx",
                ShippingAddress = "xxx-xxx"
            };

            app.Pay(payment);
        }
    }
}
