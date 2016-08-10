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

            CartItemData[] cartItems = app.Get("1");
        }

        [TestMethod]
        public void Test02()
        {
            CartApp app = new CartApp();

            app.Pay(new PaymentData());
        }
    }
}
