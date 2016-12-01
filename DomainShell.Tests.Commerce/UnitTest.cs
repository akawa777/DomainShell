using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.App;

namespace DomainShell.Tests.Commerce
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            _session = null;
        }

        private ISession _session;

        [TestMethod]
        public void Test01()
        {
            CartApp app = new CartApp(_session);

            CartItemListRequest cartItemListRequest = new CartItemListRequest
            {
                CustomerId = 1
            };

            IEnumerable<CartItemListResponse> cartItemListResults = app.Execute(cartItemListRequest);

            CartPurchaseRequest cartPurchaseRequest = new CartPurchaseRequest
            {
                CustomerId = 1,
                CardCompanyId = 1,
                CardNo = 1
            };

            app.Execute(cartPurchaseRequest);
        }
    }
}
