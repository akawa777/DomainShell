using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderRepository : IOrderRepository
    {
        public OrderRepository(IMemoryConnection connection)
        {
            _connection = connection;
        }

        private IMemoryConnection _connection; 

        public OrderModel Find(string orderId, bool throwError = false)
        {
            OrderModel orderModel = _connection.Database.Get<OrderModel>().Where(x => x.OrderId == orderId).FirstOrDefault();

            if (throwError && orderModel == null) throw new Exception("order not found.");

            return orderModel;
        }

        public void Commit(OrderModel orderModel)
        {
            bool shouldDelete = orderModel.Deleted;
            bool shouldInsert = orderModel.Dirty && orderModel.RecordVersion == 0;
            bool shouldUpdate = orderModel.Dirty && orderModel.RecordVersion > 0;

            int recordVersion = orderModel.RecordVersion;
            Action validateConcurrency = () =>
            {
                OrderModel storedOrderModel = Find(orderModel.OrderId);

                Func<bool> validateForInsert = () =>
                {
                    return shouldInsert && storedOrderModel == null;
                };

                Func<bool> validateForUpdateOrDelete = () =>
                {
                    return 
                        (shouldUpdate || shouldDelete)
                        && (storedOrderModel != null && storedOrderModel.RecordVersion != recordVersion);
                };

                if (!validateForInsert() && !validateForUpdateOrDelete())
                {
                    throw new Exception("concurrency exeption.");
                }
            };

            VirtualObject<OrderModel> vOrderModel = new VirtualObject<OrderModel>(orderModel);

            vOrderModel.Set(e => e.RecordVersion, (e, p) => e.RecordVersion + 1);
            vOrderModel.Set(e => e.Dirty, (e, p) => false);

            if (shouldDelete)
            {                
                validateConcurrency();
                _connection.Database.Delete(orderModel);
            }
            else if (shouldInsert)
            {
                validateConcurrency();
                _connection.Database.Insert(orderModel);
            } 
            else if (shouldUpdate)
            {
                validateConcurrency();
                _connection.Database.Update(orderModel);
            }
            else
            {
                return;
            }

            DomainEventPublisher.Publish(orderModel);
        }
    }    
}