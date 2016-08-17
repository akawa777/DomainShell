using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.App.Shop;

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
            ShopApp app = new ShopApp();

            CartAddItemResult addResult = app.Add(new CartAddItemCommand { CustomerId = "1", ProductId = "1", Number = 1 });
            addResult = app.Add(new CartAddItemCommand{ CustomerId = "1", ProductId = "2", Number = 2 });
            addResult = app.Add(new CartAddItemCommand { CustomerId = "1", ProductId = "3", Number = 3 });

            CartRemoveItemCommand removeItem = new CartRemoveItemCommand { CustomerId = "1", CartItemId = addResult.CartItemId };

            app.Remove(removeItem);

            CartItem[] items = app.Get("1");

            Assert.AreEqual(2, items.Length);

            PaymentAmountInfo amount = app.GetPaymentAmount("1");

            PaymentCommand payment = new PaymentCommand
            {
                CustomerId = "1",
                CreditCardNo = "xxx",
                CreditCardHolder = "xxx",
                CreditCardExpirationDate = "xxx",
                ShippingAddress = "xxx-xxx"
            };

            app.Pay(payment);
        }
    }
}
