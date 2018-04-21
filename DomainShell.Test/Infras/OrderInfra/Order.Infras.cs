using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.UserDomain;
using DomainShell.Test.Domains.OrderDomain;

namespace DomainShell.Test.Infras.OrderInfra
{    
    public class OrderRepository : IOrderRepository, IOrderReadRepository
    {
        public OrderRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;         

        Order IOrderRepository.Find(int orderId)
        {  
            var readSet = Read(command =>
            {
                var whereSqls = new List<string>();               
                var orderSqls = new List<string>();               

                SetWhereByOrderId(orderId, command, whereSqls);
                SetOrderByOrderId(orderSqls);

                return (whereSqls, orderSqls);
            });

            var order = MapToOrder(readSet).FirstOrDefault();

            return order;
        }

        Order IOrderRepository.GetLastByUser(string userId)
        {
            var readSet = Read(command =>
            {
                var whereSqls = new List<string>();
                var orderSqls = new List<string>();

                SetWhereByUser(userId, command, whereSqls);
                SetOrderByOrderId(orderSqls, "desc");

                return (whereSqls, orderSqls);
            });

            var order = MapToOrder(readSet).FirstOrDefault();     

            return order;
        }

        void IOrderRepository.Save(Order order)
        {
            if (!order.State.Modified()) return;

            if (order.Deleted)
            {
                Delete(order);
            }
            else if (order.OrderId == 0)
            {
                Insert(order);
            }
            else
            {
                Update(order);
            }
        }

        OrderRead IOrderReadRepository.Find(int orderId)
        {  
            var readSet = Read(command =>
            {
                var whereSqls = new List<string>();               
                var orderSqls = new List<string>();               

                SetWhereByOrderId(orderId, command, whereSqls);
                SetOrderByOrderId(orderSqls);

                return (whereSqls, orderSqls);
            });

            var orderRead = MapToOrderRead(readSet).FirstOrDefault();

            return orderRead;
        }

        private void SetWhereByOrderId(int orderId, IDbCommand command, List<string> whereSqls)
        {
            var whereSql = $"OrderId = @{nameof(orderId)}";
            whereSqls.Add(whereSql);

            var sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(orderId)}";
            sqlParam.Value = orderId;

            command.Parameters.Add(sqlParam);
        }

        private void SetWhereByUser(string userId, IDbCommand command, List<string> whereSqls)
        {
            var whereSql = $"UserId = @{nameof(userId)}";
            whereSqls.Add(whereSql);

            var sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(userId)}";
            sqlParam.Value = userId == null ? DBNull.Value : userId as object;

            command.Parameters.Add(sqlParam);
        }

        private void SetOrderByOrderId(List<string> orderSqls,  string ascOrDesc = "")
        {
            var orderSql = $"OrderId {ascOrDesc}";
            orderSqls.Add(orderSql);
        }

        private (IDataReader reader, IDbCommand command) Read(Func<IDbCommand, (IEnumerable<string> whereSql, IEnumerable<string> orderSqls)> createFilter, string andOr = "and")
        {
            var command = _connection.CreateCommand();

            var (whereSql, orderSqls) = createFilter(command);

            var sql = $@"
                select * from OrderForm
                where {string.Join($" {andOr} ", whereSql)}
                order by {string.Join(", ", orderSqls)}
            ";            

            command.CommandText = sql;

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<Order> MapToOrder((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                var reader = readSet.reader;

                while (reader.Read())
                {
                    ProxyObject<Order> orderProxyObject;                    

                    var order = Order.NewOrder();
                    orderProxyObject = new ProxyObject<Order>(order);

                    orderProxyObject
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PaymentId, (m, p) => reader[p.Name]);

                    yield return orderProxyObject.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<OrderRead> MapToOrderRead((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                var reader = readSet.reader;

                while (reader.Read())
                {
                    ProxyObject<OrderRead> orderReadProxyObject;                    

                    var orderRead = OrderRead.Create();
                    orderReadProxyObject = new ProxyObject<OrderRead>(orderRead);

                    orderReadProxyObject
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])                        
                        .Set(m => m.PaymentId, (m, p) => reader[p.Name]);

                    yield return orderReadProxyObject.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<(string Name, object Value)> GetParameters(Order Order)
        {
            var x = Order;

            yield return (nameof(x.UserId), x.UserId);
            yield return (nameof(x.OrderDate), x.OrderDate.Value.ToString("yyyyMMdd"));
            yield return (nameof(x.ProductName), x.ProductName);
            yield return (nameof(x.Price), x.Price);
            yield return (nameof(x.CreditCardCode), x.CreditCardCode == null ? DBNull.Value : x.CreditCardCode as object);
            yield return (nameof(x.PaymentId), x.PaymentId == null ? DBNull.Value : x.PaymentId as object);
        }
        
        private void AddParams(IDbCommand command, IEnumerable<(string Name, object Value)> parameters)
        {
            foreach (var (Name, Value) in parameters)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{Name}";
                sqlParam.Value = Value;

                command.Parameters.Add(sqlParam);
            }
        }

        private void AddOrderIdParam(IDbCommand command, Order Order)
        {
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(Order.OrderId)}";
            sqlParam.Value = Order.OrderId;

            command.Parameters.Add(sqlParam);
        }

        private void Insert(Order Order)
        {
            var parameters = GetParameters(Order);

            var sql = $@"
                insert into OrderForm (
                    {string.Join(", ", parameters.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", parameters.Select(x => $"@{x.Name}"))}
                )
            ";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                command.ExecuteNonQuery();
            }
        }

        private void Update(Order Order)
        {
            var parameters = GetParameters(Order);

            var sql = $@"
                update OrderForm 
                set
                    {string.Join(", ", parameters.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(Order.OrderId)}
            ";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                AddOrderIdParam(command, Order);
                command.ExecuteNonQuery();
            }
        }

        private void Delete(Order Order)
        {
            var sql = $@"
                delete from OrderForm                 
                where
                    OrderId = @{nameof(Order.OrderId)}
            ";

            using(var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddOrderIdParam(command, Order);
                command.ExecuteNonQuery();
            }
        }
    }  
}
