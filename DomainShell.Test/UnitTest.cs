using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using DomainShell.Test.App;

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

                OrderDto order = new OrderDto
                {
                    OrderDate = "20180101",
                    ProductName = "product1",
                    Price = 999,
                    UserId = "user1"
                };

                commandApp.Pay(order, "xxxx-xxxx-xxxx-xxxx", false);  

                var lastOrder = queryApp.GetLastByUser("user1");          

                var certificate = queryApp.IssueCertificate(lastOrder.OrderId);

                var messageList = Log.MessageList;
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            Bootstrap.StartUp(Bootstrap.DatabaseType.Sqlite);

            using (ThreadScopedLifestyle.BeginScope(Bootstrap.Container)) 
            {
                OrderCommandApp commandApp = Bootstrap.Container.GetInstance<OrderCommandApp>();
                OrderQueryApp queryApp = Bootstrap.Container.GetInstance<OrderQueryApp>();

                OrderDto order = new OrderDto
                {
                    OrderDate = "20180101",
                    ProductName = "product1",
                    Price = 999,
                    UserId = "user1"
                };

                commandApp.Pay(order, "xxxx-xxxx-xxxx-xxxx", true);  

                var messageList = Log.MessageList;
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
        public void TestMethod_ProxyObject()
        {
            ProxyObject<Root> rootProxyObject = new ProxyObject<Root>();

            rootProxyObject
                .Set(x => x.Node, (x, p) => Node.New())
                .Get(x => x.Node)
                .Set(x => x.Id, (x, p) => 999)
                .Set(x => x.Name, (x, p) => "xxx");

            ProxyObject<Node> nodeProxyObject = new ProxyObject<Node>();

            nodeProxyObject
                .Set(x => x.Id, (x, p) => 999)
                .Set(x => x.Name, (x, p) => "xxx");

            rootProxyObject
                .Set(x => x.Nodes, (x, p) => new Node[] { nodeProxyObject.Material });

            foreach(var itemProxyObject in rootProxyObject.List(x => x.Nodes))
            {
                itemProxyObject.Set(x => x.Name, (x, p) => "zzz") ;
            }
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
