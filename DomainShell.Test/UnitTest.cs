using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using DomainShell.Test.Apps;

namespace DomainShell.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestMethod()
        {
            Bootstrap.StartUp(Bootstrap.DatabaseType.Sqlite);

            using (ThreadScopedLifestyle.BeginScope(Bootstrap.Container)) 
            {
                OrderCommandApp commandApp = Bootstrap.Container.GetInstance<OrderCommandApp>();
                OrderQueryApp queryApp = Bootstrap.Container.GetInstance<OrderQueryApp>();

                OrderDto order = new OrderDto();
                order.OrderDate = "20171001";
                order.ProductName = "product1";
                order.Price = 999;
                order.UserId = "user1";

                OrderDto order2 = new OrderDto();
                order2.OrderDate = "20171001";
                order2.ProductName = "product2";
                order2.Price = 999;
                order2.UserId = "user1";

                OrderDto order3 = new OrderDto();
                order3.OrderDate = "20171001";
                order3.ProductName = "product1";
                order3.Price = 999;
                order3.UserId = "user2";

                commandApp.Register(order);
                commandApp.Register(order2);
                commandApp.Register(order3);

                order = queryApp.GetLastByUser("user1");     

                commandApp.Complete(order, "user1");

                order = queryApp.Find(order.OrderId);

                order = queryApp.GetLastByUser("user2");

                commandApp.Cancel(order);                

                order = queryApp.GetCanceledByOrderId(order.OrderId);
            }
        }

        [TestMethod]
        public void TestMethod_SQlite()
        {
            var connection = new Microsoft.Data.Sqlite.SqliteConnection("Data Source=:memory:");

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "create table OrderForm (OrderFormId integer primary key , UserId string)";
            command.ExecuteNonQuery();

            command.CommandText = "insert into OrderForm (UserId) values('xxx')";
            command.ExecuteNonQuery();

            command.CommandText = "select * from OrderForm";
            var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
            }

            connection.Close();
        }


        [TestMethod]
        public void TestMethod_VirtualObject()
        {
            Root root = new Root();

            var vRoot = new VirtualObject<Root>(root);

            vRoot.Get(m => m.Node)
                .Set(m => m.Id, (m, p) => 999)
                .Set(m => m.Name, (m, p) => "xxx");

            var nodeList = new List<Node>();

            int loopNo = 0;           
            foreach (var vNodeItem in vRoot.List(m => m.Nodes))
            {
                loopNo++;

                vNodeItem
                    .Set(m => m.Id, (m, p) => loopNo)
                    .Set(m => m.Name, (m, p) => $"xxx_{loopNo}");

                var vNode = new VirtualObject<Node>();

                vNode
                    .Set(m => m.Id, (m, p) => loopNo)
                    .Set(m => m.Name, (m, p) => $"xxx_{loopNo}");

                nodeList.Add(vNode.Material);
            }

            vRoot
                .Set("PrivateNode", (m, p) => root.Node);

            vRoot
                .Get<Node>("PrivateNode")
                .Set(m => m.Name, (m, p) => $"private {m.Name}");

            vRoot
                .Set(m => m.NodeList, (m, p) => nodeList);

            vRoot
                .Set("PrivateNodeList", (m, p) => nodeList);

            foreach (var vNodeItem in vRoot.List<Node>("PrivateNodeList"))
            {
                vNodeItem
                    .Set(m => m.Name, (m, p) => $"private {m.Name}");
            }

            vRoot
                .Set(m => m.NodeCollection, (m, p) => nodeList);            
        }

        public class Root
        {
            public Root()
            {
                Node = Node.New();
                Nodes = new Node[] { Node.New(), Node.New(), Node.New() };                
            }

            public Node Node { get; private set; }
            public Node[] Nodes { get; private set; }
            public List<Node> NodeList { get; private set; }
            private Node PrivateNode { get; set; }  
            private List<Node> PrivateNodeList { get; set; }
            public IEnumerable<Node> ReadOnlyNodes { get { return PrivateNodeList; } }
            private List<Node> _nodeCollection = null;
            public IEnumerable<Node> NodeCollection 
            { 
                get 
                { 
                    return _nodeCollection; 
                } 
                private set
                {
                    _nodeCollection = value.ToList();
                }
            }
        }

        [TestMethod]
        public void TestMethod_ShadowObject()
        {
            ProxyMaterial<Root> proxyRoot = new ProxyMaterial<Root>();

            proxyRoot.Target(x => x.Node).Value = Node.New();
            proxyRoot.Target(x => x.Node).Target(x => x.Id).Value = 1;            

            ProxyMaterial<Node> proxyNode = new ProxyMaterial<Node>();
            proxyNode.Target(x => x.Id).Value = 1;

            proxyRoot.Target(x => x.NodeList).Value = new List<Node>();
            proxyRoot.Target(x => x.NodeList).Value.Add(proxyNode.Material);
        }

        public class Node
        {
            protected Node()
            {

            }

            public static Node New()
            {
                return new Node();
            }
            public int Id { get; private set; }
            public string Name { get; private set; }
        }
    }
}
