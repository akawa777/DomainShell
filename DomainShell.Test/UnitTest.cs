using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DomainShell.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            using (ThreadScopedLifestyle.BeginScope(Bootstrap.Container)) 
            {
                OrderCommandApp commandApp = Bootstrap.Container.GetInstance<OrderCommandApp>();
                OrderQueryApp queryApp = Bootstrap.Container.GetInstance<OrderQueryApp>();

                OrderDto order = new OrderDto();
                order.ProductName = "xxx";
                order.Price = 999;
                order.LastUserId = "xxx";

                commandApp.Register(order);

                order = queryApp.GetLastByUser("xxx");

                commandApp.Complete(order, "xxx");

                order = queryApp.Find(order.OrderId);
            }
        }
    }
}
