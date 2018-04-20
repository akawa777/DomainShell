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
    public class OrderRepository : WriteRepository<Order>, IOrderRepository
    {
        public OrderRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;         

        public Order Find(int orderId, bool throwError = false)
        {  
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();               
                List<string> orderSqls = new List<string>();               

                SetWhereByOrderId(orderId, command, whereSqls);
                SetOrderByOrderId(orderSqls);

                return (whereSqls, orderSqls);
            });

            Order Order = Map(readSet).FirstOrDefault();

            if (throwError && Order == null) throw new Exception("order not found.");

            return Order;
        }

        public Order GetLastByUser(string userId)
        {
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();
                List<string> orderSqls = new List<string>();

                SetWhereByUser(userId, command, whereSqls);
                SetOrderByOrderId(orderSqls, "desc");

                return (whereSqls, orderSqls);
            });

            Order Order = Map(readSet).FirstOrDefault();

            return Order;
        }

        private void SetWhereByOrderId(int orderId, IDbCommand command, List<string> whereSqls)
        {
            string whereSql = $"OrderId = @{nameof(orderId)}";
            whereSqls.Add(whereSql);

            IDataParameter sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(orderId)}";
            sqlParam.Value = orderId;

            command.Parameters.Add(sqlParam);
        }

        private void SetWhereByUser(string userId, IDbCommand command, List<string> whereSqls)
        {
            string whereSql = $"UserId = @{nameof(userId)}";
            whereSqls.Add(whereSql);

            IDataParameter sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(userId)}";
            sqlParam.Value = userId == null ? DBNull.Value : userId as object;

            command.Parameters.Add(sqlParam);
        }

        private void SetOrderByOrderId(List<string> orderSqls,  string ascOrDesc = "")
        {
            string orderSql = $"OrderId {ascOrDesc}";
            orderSqls.Add(orderSql);
        }

        private (IDataReader reader, IDbCommand command) Read(Func<IDbCommand, (IEnumerable<string> whereSql, IEnumerable<string> orderSqls)> createFilter, string andOr = "and")
        {
            IDbCommand command = _connection.CreateCommand();

            var (whereSql, orderSqls) = createFilter(command);

            string sql = $@"
                select * from OrderForm
                where {string.Join($" {andOr} ", whereSql)}
                order by {string.Join(", ", orderSqls)}
            ";            

            command.CommandText = sql;

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<Order> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                IDataReader reader = readSet.reader;

                while (reader.Read())
                {
                    var userProxyObject = new ProxyObject<UserValue>();

                    userProxyObject
                        .Set(m => m.UserId, (m, p) => reader[p.Name]);

                    ProxyObject<Order> orderProxyObject;                    

                    if (reader["OrderId"].ToString() != "1")
                    {
                        var order = Order.NewOrder();
                        orderProxyObject = new ProxyObject<Order>(order);
                    }
                    else
                    {
                        var order = SpecialOrder.NewSpecialOrder();
                        orderProxyObject = new ProxyObject<Order>(order);
                    }

                    orderProxyObject
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.User, (m, p) => userProxyObject.Material)
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])                        
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return orderProxyObject.Material;
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
            Order x = Order;

            yield return (nameof(x.User.UserId), x.User.UserId);
            yield return (nameof(x.OrderDate), x.OrderDate.Value.ToString("yyyyMMdd"));
            yield return (nameof(x.ProductName), x.ProductName);
            yield return (nameof(x.Price), x.Price);
            yield return (nameof(x.CreditCardCode), x.CreditCardCode == null ? DBNull.Value : x.CreditCardCode as object);
            yield return (nameof(x.PayId), x.PayId == null ? DBNull.Value : x.PayId as object);
            yield return (nameof(x.RecordVersion), x.RecordVersion);            
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

        protected override void Insert(Order Order)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(Order);

            string sql = $@"
                insert into OrderForm (
                    {string.Join(", ", parameters.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", parameters.Select(x => $"@{x.Name}"))}
                )
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                command.ExecuteNonQuery();
            }
        }

        protected override void Update(Order Order)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(Order);

            string sql = $@"
                update OrderForm 
                set
                    {string.Join(", ", parameters.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(Order.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                AddOrderIdParam(command, Order);
                command.ExecuteNonQuery();
            }
        }

        protected override void Delete(Order Order)
        {
            string sql = $@"
                delete from OrderForm                 
                where
                    OrderId = @{nameof(Order.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddOrderIdParam(command, Order);
                command.ExecuteNonQuery();
            }
        }

        protected override Order Find(Order model)
        {
            return Find(model.OrderId);
        }
    }        

    public class OrderCanceledRepository : WriteRepository<OrderCanceledModel>, IOrderCanceledRepository
    {
        public OrderCanceledRepository(IConnection connection)
        {
            _connection = connection;            
        }

        private IConnection _connection;

        public OrderCanceledModel Find(int orderId, bool throwError = false)
        {
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();
                List<string> orderSqls = new List<string>();

                SetWhereByOrderId(orderId, command, whereSqls);
                SetOrderByOrderId(orderSqls);

                return (whereSqls, orderSqls);
            });

            OrderCanceledModel orderCanceledModel = Map(readSet).FirstOrDefault();

            if (throwError && orderCanceledModel == null) throw new Exception("orderCanceled not found.");

            return orderCanceledModel;
        }

        private void SetWhereByOrderId(int orderId, IDbCommand command, List<string> whereSqls)
        {
            string whereSql = $"OrderId = @{nameof(orderId)}";
            whereSqls.Add(whereSql);

            IDataParameter sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(orderId)}";
            sqlParam.Value = orderId;

            command.Parameters.Add(sqlParam);
        }

        private void SetOrderByOrderId(List<string> orderSqls, string ascOrDesc = "")
        {
            string orderSql = $"OrderId {ascOrDesc}";
            orderSqls.Add(orderSql);
        }

        private (IDataReader reader, IDbCommand command) Read(Func<IDbCommand, (IEnumerable<string> whereSql, IEnumerable<string> orderSqls)> createFilter, string andOr = "and")
        {
            IDbCommand command = _connection.CreateCommand();

            var (whereSql, orderSqls) = createFilter(command);

            string sql = $@"
                select * from OrderFormCanceled
                where {string.Join($" {andOr} ", whereSql)}
                order by {string.Join(", ", orderSqls)}
            ";

            command.CommandText = sql;

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<OrderCanceledModel> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                IDataReader reader = readSet.reader;

                while (reader.Read())
                {
                    var userProxyObject = new ProxyObject<UserValue>();

                    userProxyObject
                        .Set(m => m.UserId, (m, p) => reader[$"{p.Name}"]);

                    var orderCanceledProxyObject = new ProxyObject<OrderCanceledModel>();

                    orderCanceledProxyObject
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.User, (m, p) => userProxyObject.Material)
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])                        
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return orderCanceledProxyObject.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<(string Name, object Value)> GetParameters(OrderCanceledModel orderCanceledModel)
        {
            OrderCanceledModel x = orderCanceledModel;

            yield return (nameof(x.OrderId), x.OrderId);
            yield return (nameof(x.OrderDate), x.OrderDate.Value.ToString("yyyyMMdd"));
            yield return (nameof(x.ProductName), x.ProductName);
            yield return (nameof(x.Price), x.Price);
            yield return (nameof(x.CreditCardCode), x.CreditCardCode == null ? DBNull.Value : x.CreditCardCode as object);
            yield return (nameof(x.PayId), x.PayId == null ? DBNull.Value : x.PayId as object);
            yield return (nameof(x.RecordVersion), x.RecordVersion);
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

        private void AddOrderIdParam(IDbCommand command, OrderCanceledModel orderCanceledModel)
        {
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(orderCanceledModel.OrderId)}";
            sqlParam.Value = orderCanceledModel.OrderId;

            command.Parameters.Add(sqlParam);
        }

        protected override void Insert(OrderCanceledModel orderCanceledModel)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(orderCanceledModel);

            string sql = $@"
                insert into OrderFormCanceled (
                    {string.Join(", ", parameters.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", parameters.Select(x => $"@{x.Name}"))}
                )
            ";

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                command.ExecuteNonQuery();
            }
        }

        protected override void Update(OrderCanceledModel orderCanceledModel)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(orderCanceledModel);

            string sql = $@"
                update OrderFormCanceled 
                set
                    {string.Join(", ", parameters.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(orderCanceledModel.OrderId)}
            ";

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                AddOrderIdParam(command, orderCanceledModel);
                command.ExecuteNonQuery();
            }
        }

        protected override void Delete(OrderCanceledModel orderCanceledModel)
        {
            string sql = $@"
                delete from OrderFormCanceled                 
                where
                    OrderId = @{nameof(orderCanceledModel.OrderId)}
            ";

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddOrderIdParam(command, orderCanceledModel);
                command.ExecuteNonQuery();
            }
        }

        protected override OrderCanceledModel Find(OrderCanceledModel model)
        {
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();
                List<string> orderSqls = new List<string>();

                SetWhereByOrderId(model.OrderId, command, whereSqls);
                SetOrderByOrderId(orderSqls, "asc");

                return (whereSqls, orderSqls);
            });

            OrderCanceledModel orderCanceledModel = Map(readSet).FirstOrDefault();

            return orderCanceledModel;
        }
    }

    public class MonthlyOrderRepository : IMonthlyOrderRepository
    {
        public MonthlyOrderRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public MonthlyOrder GetMonthlyByUserId(string userId, DateTime orderDate, int excludeOrderId = 0)
        {
            string yearMonth = orderDate.Year.ToString() + orderDate.Month.ToString().PadLeft(2, '0');

            using (IDbCommand command = _connection.CreateCommand())
            {
                string sql = $@"
                    select 
                        Budget, TotalPrice, TotalOrderNo
                    from 
                        MonthlyOrderBudget
                    left join 
                        (
                            select UserId, sum(Price) TotalPrice, count(OrderId) TotalOrderNo from OrderForm
                            where 
                                UserId = @{nameof(userId)}                        
                                and OrderDate like @{nameof(yearMonth)} + '%'
                                {(excludeOrderId <= 0 ? "" : $"and OrderId != @{nameof(excludeOrderId)}")}
                            group by
								UserId
                        ) OrderForm                            
                    on 
                        MonthlyOrderBudget.UserId = OrderForm.UserId                                            
                    where 
                        MonthlyOrderBudget.UserId = @{nameof(userId)}
                ";

                command.CommandText = sql;

                IDbDataParameter sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{nameof(userId)}";
                sqlParam.Value = userId;

                command.Parameters.Add(sqlParam);

                sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{nameof(yearMonth)}";
                sqlParam.Value = yearMonth;

                command.Parameters.Add(sqlParam);

                sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{nameof(excludeOrderId)}";
                sqlParam.Value = excludeOrderId;

                command.Parameters.Add(sqlParam);

                var monthlyOrderProxyObject = new ProxyObject<MonthlyOrder>();

                monthlyOrderProxyObject
                    .Set(m => m.UserId, (m, p) => userId)
                    .Set(m => m.Year, (m, p) => orderDate.Year)
                    .Set(m => m.Month, (m, p) => orderDate.Month);

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {                        
                        monthlyOrderProxyObject                                                        
                            .Set(m => m.Budget, (m, p) => reader[p.Name])
                            .Set(m => m.TotalPrice, (m, p) => reader[p.Name] == DBNull.Value ? 0 : reader[p.Name])
                            .Set(m => m.TotalOrderNo, (m, p) => reader[p.Name] == DBNull.Value ? 0 : reader[p.Name]);
                    }
                }               

                return monthlyOrderProxyObject.Material;
            }
        }

        public object[] GetMonthlyOrderBudgets()
        {
            using (var dbContext = new DatabaseContext(_connection))
            {
                return dbContext.MonthlyOrderBudget.Join(
                    dbContext.LoginUser, 
                    x => x.UserId, 
                    x => x.UserId,
                    (main, join) => new 
                    {
                         main.UserId,
                         join.UserName,
                         main.Budget
                    }).ToArray();
            }
        }
    }
}
