using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test
{
    public class OrderRepository : IOrderRepository
    {
        public OrderRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        private IDbConnection _connection; 

        private enum ModelState
        {
            Added,
            Deleted,
            Modified,
            Unchanged
        }

        public OrderModel Find(string orderId, bool throwError = false)
        {   
            OrderModel orderModel = Map(ReadByOrderFormId(orderId)).FirstOrDefault();

            if (throwError && orderModel == null) throw new Exception("order not found.");

            return orderModel;
        }

        public OrderModel GetLastByUser(string userId)
        {
             OrderModel orderModel = Map(ReadLastByUser(userId)).FirstOrDefault();

            return orderModel;
        }

        public void Save(OrderModel orderModel)
        {
            ModelState modelState = GetModelState(orderModel);

            ValidateConcurrency(orderModel, modelState);

            AdjustWhenSave(orderModel);

            Save(orderModel, modelState);

            PublishDomainEvent(orderModel, modelState);
        }

        private ModelState GetModelState(OrderModel orderModel)
        {
            if (orderModel.Deleted) return ModelState.Deleted;
            if (orderModel.Dirty && orderModel.RecordVersion == 0) return ModelState.Added;
            if (orderModel.Dirty && orderModel.RecordVersion > 0) return ModelState.Modified;

            return ModelState.Unchanged;
        }

        private bool ValidateConcurrency(OrderModel orderModel, ModelState modelState)
        {
            OrderModel storedOrderModel = Find(orderModel.OrderId);

            if (modelState == ModelState.Added && storedOrderModel != null) return false;
            if (modelState == ModelState.Modified && storedOrderModel.RecordVersion != orderModel.RecordVersion) return false;
            if (modelState == ModelState.Deleted && storedOrderModel.RecordVersion != orderModel.RecordVersion) return false;            

            return true;
        }

        private void AdjustWhenSave(OrderModel orderModel)
        {
            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);

            vOrderModel
                .Set(m => m.RecordVersion, (m, p) => m.RecordVersion + 1)
                .Set(m => m.Dirty, (m, p) => false);
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

        private void PublishDomainEvent(OrderModel orderModel, ModelState modelState)
        {
            if (modelState == ModelState.Unchanged) return;
            else DomainEventPublisher.Publish(orderModel);
        }

        private IDataReader ReadByOrderFormId(string orderId)
        {
            string sql = $@"
                select * from OrderForm
                where OrderId = @{nameof(orderId)}
            ";

            var command = _connection.CreateCommand();

            command.CommandText = sql;

            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{nameof(orderId)}";
            parameter.Value = orderId == null ? DBNull.Value : orderId as object;

            command.Parameters.Add(parameter);

            return command.ExecuteReader();
        }

        private IDataReader ReadLastByUser(string userId)
        {
            string sql = $@"
                select * from OrderForm
                where LastUserId = @{nameof(userId)}  
                order by OrderId desc
            ";

            var command = _connection.CreateCommand();

            command.CommandText = sql;

            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{nameof(userId)}";
            parameter.Value = userId;

            command.Parameters.Add(parameter);

            return command.ExecuteReader();
        }

        private IEnumerable<OrderModel> Map(IDataReader reader)
        {
            while(reader.Read())
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

        private IEnumerable<VirtualProperty> GetVirtualSqlParams(OrderModel orderModel)
        {
            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);

            yield return vOrderModel.GetProperty(m => m.ProductName);
            yield return vOrderModel.GetProperty(m => m.Price);
            yield return vOrderModel.GetProperty(m => m.PayId, (m, p) => m.PayId == null ? DBNull.Value : m.PayId as object);
            yield return vOrderModel.GetProperty(m => m.LastUserId);
            yield return vOrderModel.GetProperty(m => m.RecordVersion);              
        }

        private void AddParams(IDbCommand command, IEnumerable<VirtualProperty> vSqlParams)
        {
            foreach (var vSqlParam in vSqlParams)
            {
                var sqlParam = command.CreateParameter();
                sqlParam.ParameterName = $"@{vSqlParam.Name}";
                sqlParam.Value = vSqlParam.value;

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
            IEnumerable<VirtualProperty> vSqlParams = GetVirtualSqlParams(orderModel);

            string sql = $@"
                insert into OrderForm (
                    {string.Join(", ", vSqlParams.Select(x => x.Name))}
                ) values (
                    {string.Join(", ", vSqlParams.Select(x => $"@{x.Name}"))}
                )
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vSqlParams);
                command.ExecuteNonQuery();
            }
        }

        private void Update(OrderModel orderModel)
        {
            IEnumerable<VirtualProperty> vSqlParams = GetVirtualSqlParams(orderModel);

            string sql = $@"
                update OrderForm 
                set
                    {string.Join(", ", vSqlParams.Select(x => $"{x.Name} = @{x.Name}"))}
                where
                    OrderId = @{nameof(orderModel.OrderId)}
            ";

            using(IDbCommand command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                AddParams(command, vSqlParams);
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
