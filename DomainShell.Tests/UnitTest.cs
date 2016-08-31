﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.App.Shop;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Domain.Customer;

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

            AddCartItemResult addCartItemResult = app.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "1", Number = 1 });
            addCartItemResult = app.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "2", Number = 2 });
            addCartItemResult = app.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "3", Number = 3 });

            RemoveCartItemCommand removeCartItemCommand = new RemoveCartItemCommand { CustomerId = "1", CartItemId = addCartItemResult.CartItemId };

            app.RemoveCartItem(removeCartItemCommand);

            CartItem[] items = app.GetCartItems(new CartItemsQuery { CustomerId = "1" });

            Assert.AreEqual(2, items.Length);            

            PayCommand payCommand = new PayCommand
            {
                CustomerId = "1",
                CreditCardNo = "xxx",
                CreditCardHolder = "xxx",
                CreditCardExpirationDate = "xxx",
                ShippingAddress = "xxx-xxx"
            };

            app.Pay(payCommand);
        }

        [TestMethod]
        public void Test02()
        {
            Session session = new Session();

            using (session.Open())
            {
                CustomerRepository repository = new CustomerRepository(session);

                CustomerModel model = repository.Find("1");

                string id = model.CustomerId;
            }
        }
    }
}
