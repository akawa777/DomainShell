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

            CartAddItemResult addResult = app.Add(new CartAddItem { CustomerId = "1", ProductId = "1", Number = 1 });
            addResult = app.Add(new CartAddItem { CustomerId = "1", ProductId = "2", Number = 2 });
            addResult = app.Add(new CartAddItem { CustomerId = "1", ProductId = "3", Number = 3 });

            CartRemoveItem removeItem = new CartRemoveItem { CustomerId = "1", CartItemId = addResult.CartItemId };

            app.Remove(removeItem);

            CartItem[] items = app.Get("1");

            Assert.AreEqual(2, items.Length);

            decimal amount = app.GetPaymentAmount(new PaymentAmountQuery { CustomerId = "1", ShippingAddress = "xxx-xxx" });

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
