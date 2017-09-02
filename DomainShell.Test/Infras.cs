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

        private enum ModelState
        {
            Added,
            Deleted,
            Modified,
            Unchanged
        }

        public OrderModel Find(string orderId, bool throwError = false)
        {
            OrderModel orderModel = _connection.Database.Get<OrderModel>().Where(x => x.OrderId == orderId).FirstOrDefault();

            if (throwError && orderModel == null) throw new Exception("order not found.");

            return orderModel;
        }

        public OrderModel GetLastByUser(string userId)
        {
             OrderModel orderModel =_connection.Database.Get<OrderModel>().Where(x => x.LastUserId == userId).LastOrDefault();

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

            vOrderModel.Set(e => e.RecordVersion, (e, p) => e.RecordVersion + 1);
            vOrderModel.Set(e => e.Dirty, (e, p) => false);
        }

        private void Save(OrderModel orderModel, ModelState modelState)
        {
            if (modelState == ModelState.Deleted)
            {
                _connection.Database.Delete(orderModel);
            }
            else if (modelState == ModelState.Added)
            {
                _connection.Database.Insert(orderModel);
            }
            else if (modelState == ModelState.Modified)
            {
                _connection.Database.Update(orderModel);
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
    }    
}