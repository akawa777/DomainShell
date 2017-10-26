using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.User;
using DomainShell.Test.Domains.Order;

namespace DomainShell.Test.Infras.Order
{    
    public class OrderRepository : WriteRepository<OrderModel>, IOrderRepository
    {
        public OrderRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;         

        public OrderModel Find(int orderId, bool throwError = false)
        {  
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();               
                List<string> orderSqls = new List<string>();               

                SetWhereByOrderId(orderId, command, whereSqls);
                SetOrderByOrderId(orderSqls);

                return (whereSqls, orderSqls);
            });

            OrderModel orderModel = Map(readSet).FirstOrDefault();

            if (throwError && orderModel == null) throw new Exception("order not found.");

            return orderModel;
        }

        public OrderModel GetLastByUser(string userId)
        {
            var readSet = Read(command =>
            {
                List<string> whereSqls = new List<string>();
                List<string> orderSqls = new List<string>();

                SetWhereByUser(userId, command, whereSqls);
                SetOrderByOrderId(orderSqls, "desc");

                return (whereSqls, orderSqls);
            });

            OrderModel orderModel = Map(readSet).FirstOrDefault();

            return orderModel;
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

            var filter = createFilter(command);

            string sql = $@"
                select * from OrderForm
                where {string.Join($" {andOr} ", filter.whereSql)}
                order by {string.Join(", ", filter.orderSqls)}
            ";            

            command.CommandText = sql;

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<OrderModel> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                IDataReader reader = readSet.reader;

                while (reader.Read())
                {
                    var userSurrogate = new Surrogate<UserValue>();

                    userSurrogate
                        .Set(m => m.UserId, (m, p) => reader[p.Name]);

                    var orderModel = DomainModelProxyFactory.Create<OrderModel>();
                    var orderSurrogate = new Surrogate<OrderModel>(orderModel);

                    orderSurrogate
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.User, (m, p) => userSurrogate.Material)
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])                        
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return orderSurrogate.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<(string Name, object Value)> GetParameters(OrderModel orderModel)
        {
            OrderModel x = orderModel;

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
            foreach (var parameter in parameters)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{parameter.Name}";
                sqlParam.Value = parameter.Value;

                command.Parameters.Add(sqlParam);
            }
        }

        private void AddOrderIdParam(IDbCommand command, OrderModel orderModel)
        {
            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(orderModel.OrderId)}";
            sqlParam.Value = orderModel.OrderId;

            command.Parameters.Add(sqlParam);
        }

        protected override void Insert(OrderModel orderModel)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(orderModel);

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

        protected override void Update(OrderModel orderModel)
        {
            IEnumerable<(string Name, object Value)> parameters = GetParameters(orderModel);

            string sql = $@"
                update OrderForm 
                set
                    {string.Join(", ", parameters.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(orderModel.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, parameters);
                AddOrderIdParam(command, orderModel);
                command.ExecuteNonQuery();
            }
        }

        protected override void Delete(OrderModel orderModel)
        {
            string sql = $@"
                delete from OrderForm                 
                where
                    OrderId = @{nameof(orderModel.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddOrderIdParam(command, orderModel);
                command.ExecuteNonQuery();
            }
        }

        protected override OrderModel Find(OrderModel model)
        {
            return Find(model.OrderId);
        }
    }    

    public class OrderModelProxy : OrderModel, IDomainModelProxy
    {
        public override void Complete(ICreditCardService creditCardService, string creditCardCode)
        {
            OutputLog($"{nameof(OrderModelProxy)} {nameof(Complete)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            
            base.Complete(creditCardService, creditCardCode);
        }

        public Type GetImplementType()
        {
            return typeof(OrderModel);
        }

        private void OutputLog(string message)
        {
            Console.WriteLine($"logging {message}");
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

            var filter = createFilter(command);

            string sql = $@"
                select * from OrderFormCanceled
                where {string.Join($" {andOr} ", filter.whereSql)}
                order by {string.Join(", ", filter.orderSqls)}
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
                    var userSurrogate = new Surrogate<UserValue>();

                    userSurrogate
                        .Set(m => m.UserId, (m, p) => reader[$"{p.Name}"]);

                    var orderCanceledSurrogate = new Surrogate<OrderCanceledModel>();

                    orderCanceledSurrogate
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.OrderDate, (m, p) => DateTime.ParseExact(reader[p.Name].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
                        .Set(m => m.User, (m, p) => userSurrogate.Material)
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])                        
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return orderCanceledSurrogate.Material;
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
            foreach (var parameter in parameters)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{parameter.Name}";
                sqlParam.Value = parameter.Value;

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

        public MonthlyOrderModel GetMonthlyByUserId(string userId, DateTime orderDate, int excludeOrderId = 0)
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

                var monthlyOrderSurrogate = new Surrogate<MonthlyOrderModel>();

                monthlyOrderSurrogate
                    .Set(m => m.UserId, (m, p) => userId)
                    .Set(m => m.Year, (m, p) => orderDate.Year)
                    .Set(m => m.Month, (m, p) => orderDate.Month);

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {                        
                        monthlyOrderSurrogate                                                        
                            .Set(m => m.Budget, (m, p) => reader[p.Name])
                            .Set(m => m.TotalPrice, (m, p) => reader[p.Name] == DBNull.Value ? 0 : reader[p.Name])
                            .Set(m => m.TotalOrderNo, (m, p) => reader[p.Name] == DBNull.Value ? 0 : reader[p.Name]);
                    }
                }               

                return monthlyOrderSurrogate.Material;
            }
        }
    }
}
