using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Apps.Cart;
using DomainShell.Tests.Apps.Payment;

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

            List<CartData> cartList = app.GetAll();
        }

        [TestMethod]
        public void Test02()
        {
            PaymentApp app = new PaymentApp();            

            app.Pay(new PaymentData
            {
                CartId = "1",
                Postage = 100
            });
        }
    }
}
