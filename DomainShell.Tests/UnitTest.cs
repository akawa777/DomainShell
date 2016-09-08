using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.App.Shop;
using DomainShell.Tests.App.Cart;
using DomainShell.Tests.App.Purchase;

namespace DomainShell.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {   
            SqliteSessionKernel.Config(DataStoreProvider.CreateConnection);
        }

        [TestMethod]
        public void Test01()
        {
            ShopApp shopApp = new ShopApp();
            CartApp cartApp = new CartApp();
            PurchaseApp purchaseApp = new PurchaseApp();

            AddCartItemResult addCartItemResult = shopApp.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "1", Number = 1 });
            addCartItemResult = shopApp.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "2", Number = 2 });
            addCartItemResult = shopApp.AddCartItem(new AddCartItemCommand { CustomerId = "1", ProductId = "3", Number = 3 });

            RemoveCartItemCommand removeCartItemCommand = new RemoveCartItemCommand { CustomerId = "1", CartItemId = addCartItemResult.CartItemId };

            cartApp.RemoveCartItem(removeCartItemCommand);

            CartItem[] items = cartApp.GetCartItems(new CartItemsQuery { CustomerId = "1" });

            Assert.AreEqual(2, items.Length);

            CheckoutCommand checkoutCommand = new CheckoutCommand
            {
                CustomerId = "1",
                CreditCardNo = "xxx",
                CreditCardHolder = "xxx",
                CreditCardExpirationDate = "xxx",
                ShippingAddress = "xxx-xxx"
            };

            cartApp.Checkout(checkoutCommand);
        }

        [TestMethod]
        public void Test02()
        {
            SqliteSessionKernel kernel = new SqliteSessionKernel();

            Session session = new Session(kernel);

            CustomerRepository repository = new CustomerRepository(session);

            using (session.Connect())
            {
                CustomerModel model = repository.Find("1");

                string id = model.CustomerId;
            }
            
            using (ITran tran = session.Tran())
            {
                CustomerModel model = repository.Find("1");

                string id = model.CustomerId;

                tran.Complete();
            }
        }
    }
}
