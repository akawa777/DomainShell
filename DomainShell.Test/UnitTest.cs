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
                order.ProductName = "x";
                order.Price = 100m;

                string orderId = commandApp.Regist(order);     

                order = queryApp.Find(orderId);

                commandApp.Complete(order, "9999");

                order = queryApp.Find(order.OrderId);
            }
        }
    }
}
