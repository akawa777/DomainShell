using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test
{
    public class OrderRepository : IOrderRepository, IWriteRepository<OrderModel>
    {
        public OrderRepository(IConnection connection)
        {
            _connection = connection;
        }

        private IConnection _connection;         

        public OrderModel Find(string orderId, bool throwError = false)
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

        public OrderModel GetStored(OrderModel orderModel)
        {
            return Find(orderModel.OrderId);
        }

        private void SetWhereByOrderId(string orderId, IDbCommand command, List<string> whereSqls)
        {
            string whereSql = $"OrderId = @{nameof(orderId)}";
            whereSqls.Add(whereSql);

            IDataParameter sqlParam = command.CreateParameter();

            sqlParam.ParameterName = $"@{nameof(orderId)}";
            sqlParam.Value = orderId == null ? DBNull.Value : orderId as object;

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
                    var vOrderModel = new VirtualObject<OrderModel>();

                    vOrderModel
                        .Set(m => m.OrderId, (m, p) => reader[p.Name])
                        .Set(m => m.ProductName, (m, p) => reader[p.Name])
                        .Set(m => m.Price, (m, p) => reader[p.Name])
                        .Set(m => m.PayId, (m, p) => reader[p.Name])
                        .Set(m => m.LastUserId, (m, p) => reader[p.Name])
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

        private IEnumerable<VirtualProperty> GetVirtualProperties(OrderModel orderModel, bool includePrimaryKey = false)
        {
            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);

            if (includePrimaryKey) vOrderModel.GetProperty(m => m.OrderId);
            yield return vOrderModel.GetProperty(m => m.ProductName);
            yield return vOrderModel.GetProperty(m => m.Price);
            yield return vOrderModel.GetProperty(m => m.PayId, (m, p) => m.PayId == null ? DBNull.Value : m.PayId as object);
            yield return vOrderModel.GetProperty(m => m.LastUserId);
            yield return vOrderModel.GetProperty(m => m.RecordVersion);              
        }

        public void Save(OrderModel orderModel)
        {
            ModelState modelState = GetModelState(orderModel);

            ValidateConcurrency(orderModel, modelState);

            AdjustWhenSave(orderModel, modelState);

            Save(orderModel, modelState);

            AddDomainEvents(orderModel, modelState);
        }

        private ModelState GetModelState(OrderModel orderModel)
        {
            if (orderModel.Dirty.Is && orderModel.Deleted.Is && orderModel.RecordVersion > 0) return ModelState.Deleted;
            if (orderModel.Dirty.Is && orderModel.RecordVersion == 0) return ModelState.Added;
            if (orderModel.Dirty.Is && orderModel.RecordVersion > 0) return ModelState.Modified;

            return ModelState.Unchanged;
        }

        private void ValidateConcurrency(OrderModel orderModel, ModelState modelState)
        {
            OrderModel storedOrderModel = Find(orderModel.OrderId);

            bool valid = true;

            if (modelState == ModelState.Added && storedOrderModel != null) valid = false;
            if (modelState == ModelState.Modified && storedOrderModel.RecordVersion != orderModel.RecordVersion) valid = false;
            if (modelState == ModelState.Deleted && storedOrderModel.RecordVersion != orderModel.RecordVersion) valid = false;

            if (!valid) throw new Exception("concurrency exception.");
        }

        private void AdjustWhenSave(OrderModel orderModel, ModelState modelState)
        {
            if (modelState == ModelState.Unchanged) return;

            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);

            vOrderModel
                .Set(m => m.RecordVersion, (m, p) => m.RecordVersion + 1)
                .Set(m => m.Dirty, (m, p) => Dirty.False());
        }

        private void Save(OrderModel orderModel, ModelState modelState)
        {
            if (modelState == ModelState.Deleted)
            {
                Delete(orderModel);
            }
            else if (modelState == ModelState.Added)
            {
                Insert(orderModel);
            }
            else if (modelState == ModelState.Modified)
            {
                Update(orderModel);
            }
            else
            {
                return;
            }
        }

        private void AddDomainEvents(OrderModel orderModel, ModelState modelState)
        {
            if (modelState == ModelState.Unchanged) return;
            else DomainEventList.Add(orderModel);
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

        private void Insert(OrderModel orderModel)
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

        private void Update(OrderModel orderModel)
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

        private void Delete(OrderModel orderModel)
        {
            string sql = $@"
                delete from OrderForm                 
                where
                    OrderId = @{nameof(orderModel.OrderId)},
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddOrderIdParam(command, orderModel);
                command.ExecuteNonQuery();
            }
        }
    }    
}
