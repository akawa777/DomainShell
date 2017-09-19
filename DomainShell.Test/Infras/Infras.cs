using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using DomainShell.Test.Domains;

namespace DomainShell.Test.Infras
{
    public abstract class WriteRepository<TAggregateRoot> : IWriteRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        public void Save(TAggregateRoot model)
        {
            ModelState modelState = GetModelState(model);

            ValidateConcurrency(model, modelState);

            AdjustWhenSave(model, modelState);

            Save(model, modelState);
        }

        private ModelState GetModelState(TAggregateRoot model)
        {
            if (!model.Dirty.Is()) return ModelState.Unchanged;
            if (model.Dirty.Is() && model.Deleted) return ModelState.Deleted;
            if (model.Dirty.Is() && model.RecordVersion == 0) return ModelState.Added;
            else return ModelState.Modified;
        }

        private void ValidateConcurrency(TAggregateRoot model, ModelState modelState)
        {
            TAggregateRoot storedModel = Find(model);

            bool valid = true;

            if (modelState == ModelState.Added && storedModel != null) valid = false;
            if (modelState == ModelState.Modified && storedModel.RecordVersion != model.RecordVersion) valid = false;
            if (modelState == ModelState.Deleted && storedModel.RecordVersion != model.RecordVersion) valid = false;

            if (!valid) throw new Exception("concurrency exception.");
        }

        private void AdjustWhenSave(TAggregateRoot model, ModelState modelState)
        {
            if (modelState == ModelState.Unchanged) return;

            VirtualObject<TAggregateRoot> vModel = new VirtualObject<TAggregateRoot>(model);

            vModel
                .Set(m => m.RecordVersion, (m, p) => m.RecordVersion + 1)
                .Set(m => m.Dirty, (m, p) => Dirty.Clear(m));
        }

        protected abstract TAggregateRoot Find(TAggregateRoot model);

        protected void Save(TAggregateRoot model, ModelState modelState)
        {
            if (modelState == ModelState.Deleted)
            {
                Delete(model);
            }
            else if (modelState == ModelState.Added)
            {
                Insert(model);
            }
            else if (modelState == ModelState.Modified)
            {
                Update(model);
            }
            else
            {
                return;
            }
        }

        protected abstract void Insert(TAggregateRoot model);

        protected abstract void Update(TAggregateRoot model);

        protected abstract void Delete(TAggregateRoot model);
    }

    public class UserRepository : IUserRepository
    {
        public UserRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public UserModel Find(string userId, bool throwError = false)
        {
            var readSet = Read(userId);

            UserModel userModel = Map(readSet).FirstOrDefault();

            if (throwError && userModel == null) throw new Exception("user not found.");

            return userModel;
        }

        private (IDataReader reader, IDbCommand command) Read(string userId)
        {
            IDbCommand command = _connection.CreateCommand();

            string sql = $@"
                select * from LoginUser
                where UserId = @userId
            ";

            command.CommandText = sql;

            var sqlParam = command.CreateParameter();
            sqlParam.ParameterName = $"@{nameof(userId)}";
            sqlParam.Value = userId == null ? DBNull.Value : userId as object;

            command.Parameters.Add(sqlParam);

            return (command.ExecuteReader(), command);
        }

        private IEnumerable<UserModel> Map((IDataReader reader, IDbCommand command) readSet)
        {
            try
            {
                IDataReader reader = readSet.reader;

                while (reader.Read())
                {
                    var vUserModel = new VirtualObject<UserModel>();

                    vUserModel
                        .Set(m => m.UserId, (m, p) => reader[p.Name])
                        .Set(m => m.UserName, (m, p) => reader[p.Name])
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return vUserModel.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }
    }

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

                SetWhereByLastUser(userId, command, whereSqls);
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

        private void SetWhereByLastUser(string userId, IDbCommand command, List<string> whereSqls)
        {
            string whereSql = $"LastUserId = @{nameof(userId)}";
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
                    var vUserValue = new VirtualObject<UserValue>();

                    vUserValue
                        .Set(m => m.UserId, (m, p) => reader[$"Last{p.Name}"]);

                    var orderModel = DomainModelProxyFactory.Create<OrderModel>();
                    var vOrderModel = new VirtualObject<OrderModel>(orderModel);

                    vOrderModel
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])
                        .Set(m => m.LastUser, (m, p) => vUserValue.Material)
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return vOrderModel.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<VirtualProperty> GetVirtualProperties(OrderModel orderModel)
        {
            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);
            
            yield return vOrderModel.GetProperty(m => m.ProductName);
            yield return vOrderModel.GetProperty(m => m.Price);
            yield return vOrderModel.GetProperty(m => m.CreditCardCode, getValue: (m, p) => m.CreditCardCode == null ? DBNull.Value : m.CreditCardCode as object);
            yield return vOrderModel.GetProperty(m => m.PayId, getValue: (m, p) => m.PayId == null ? DBNull.Value : m.PayId as object);
            yield return vOrderModel.Get(x => x.LastUser).GetProperty(m => m.UserId, getName: (m, p) => $"Last{p.Name}");
            yield return vOrderModel.GetProperty(m => m.RecordVersion);              
        }
        
        private void AddParams(IDbCommand command, IEnumerable<VirtualProperty> virtualProperties)
        {
            foreach (var vProp in virtualProperties)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{vProp.Name}";
                sqlParam.Value = vProp.Value;

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
            IEnumerable<VirtualProperty> vProps = GetVirtualProperties(orderModel);

            string sql = $@"
                insert into OrderForm (
                    {string.Join(", ", vProps.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", vProps.Select(x => $"@{x.Name}"))}
                )
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vProps);
                command.ExecuteNonQuery();
            }
        }

        protected override void Update(OrderModel orderModel)
        {
            IEnumerable<VirtualProperty> vProps = GetVirtualProperties(orderModel);

            string sql = $@"
                update OrderForm 
                set
                    {string.Join(", ", vProps.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(orderModel.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vProps);
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
        public override void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
        {
            OutputLog($"{nameof(OrderModelProxy)} {nameof(Complete)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            
            base.Complete(orderValidator, creditCardService, creditCardCode);
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
                    var vUserValue = new VirtualObject<UserValue>();

                    vUserValue
                        .Set(m => m.UserId, (m, p) => reader[$"Last{p.Name}"]);

                    var vOrderCanceledModel = new VirtualObject<OrderCanceledModel>();

                    vOrderCanceledModel
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.CreditCardCode, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])
                        .Set(m => m.LastUser, (m, p) => vUserValue.Material)
                        .Set(m => m.RecordVersion, (m, p) => reader[p.Name]);

                    yield return vOrderCanceledModel.Material;
                }
            }
            finally
            {
                readSet.reader.Dispose();
                readSet.command.Dispose();
            }
        }

        private IEnumerable<VirtualProperty> GetVirtualProperties(OrderCanceledModel orderCanceledModel)
        {
            VirtualObject<OrderCanceledModel> vOrderCanceledModel = new VirtualObject<OrderCanceledModel>(orderCanceledModel);

            yield return vOrderCanceledModel.GetProperty(m => m.OrderId);
            yield return vOrderCanceledModel.GetProperty(m => m.ProductName);
            yield return vOrderCanceledModel.GetProperty(m => m.Price);
            yield return vOrderCanceledModel.GetProperty(m => m.CreditCardCode, getValue: (m, p) => m.CreditCardCode == null ? DBNull.Value : m.CreditCardCode as object);
            yield return vOrderCanceledModel.GetProperty(m => m.PayId, getValue: (m, p) => m.PayId == null ? DBNull.Value : m.PayId as object);
            yield return vOrderCanceledModel.Get(x => x.LastUser).GetProperty(m => m.UserId, getName: (m, p) => $"Last{p.Name}");
            yield return vOrderCanceledModel.GetProperty(m => m.RecordVersion);
        }

        private void AddParams(IDbCommand command, IEnumerable<VirtualProperty> virtualProperties)
        {
            foreach (var vProp in virtualProperties)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{vProp.Name}";
                sqlParam.Value = vProp.Value;

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
            IEnumerable<VirtualProperty> vProps = GetVirtualProperties(orderCanceledModel);

            string sql = $@"
                insert into OrderFormCanceled (
                    {string.Join(", ", vProps.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", vProps.Select(x => $"@{x.Name}"))}
                )
            ";

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vProps);
                command.ExecuteNonQuery();
            }
        }

        protected override void Update(OrderCanceledModel orderCanceledModel)
        {
            IEnumerable<VirtualProperty> vProps = GetVirtualProperties(orderCanceledModel);

            string sql = $@"
                update OrderFormCanceled 
                set
                    {string.Join(", ", vProps.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(orderCanceledModel.OrderId)}
            ";

            using (IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vProps);
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

    public class OrderSummaryReader : IOrderSummaryReader
    {
        public OrderSummaryReader(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;

        public IEnumerable<OrderSummaryValue> GetSummary()
        {
            using (IDbCommand command = _connection.CreateCommand())
            {
                string sql = $@"
                    select ProductName, sum(Price) TotalPrice, count(OrderId) TotalOrderNo from OrderForm                
                    group by ProductName
                    order by ProductName                    
                ";

                command.CommandText = sql;

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {                        
                        var vOrderSummaryValue = new VirtualObject<OrderSummaryValue>();

                        vOrderSummaryValue
                            .Set(m => m.ProductName, (m, p) => reader[p.Name])
                            .Set(m => m.TotalPrice, (m, p) => reader[p.Name])
                            .Set(m => m.TotalOrderNo, (m, p) => reader[p.Name]);

                        yield return vOrderSummaryValue.Material;
                    }
                }
            }
        }
    }
}
