using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderModel : IAggregateRoot
    {
        public static OrderModel NewOrder()
        {
            OrderModel orderModel = DomainModelProxyFactory.Create<OrderModel>();
            
            return orderModel;
        }

        protected OrderModel()
        {
            Dirty = Dirty.False();
            Deleted = Deleted.False();

            DomainModelMarker.Mark(this);
        }

        public int OrderId { get; private set; }               

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; private set; }

        public string LastUserId { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public Deleted Deleted { get; private set; }

        private List<IDomainEvent> _events = new List<IDomainEvent>();
        
        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public void Register(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenRegister(this);

            Dirty = Dirty.True();
        }

        public void Cancel(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenCancel(this);

            Dirty = Dirty.True();

            Deleted = Deleted.True();

            AddCanceledEvents();
        }

        public virtual void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
        {
            orderValidator.ValidateWhenComplete(this);

            Dirty = Dirty.True();

            Pay(creditCardService, creditCardCode);

            AddCompletedEvents();
        }

        public void SendCompletedMail(IMailService mailService)
        {
            mailService.Send("xxx", "xxx", "xxx");
        }

        public void CancelCompleted(ICreditCardService creditCardService)
        {
            Dirty = Dirty.True();

            CancelPay(creditCardService);
        }

        private void Pay(ICreditCardService creditCardService, string creditCardCode)
        {
            PayId = creditCardService.Pay(creditCardCode, Price);
        }

        private void CancelPay(ICreditCardService creditCardService)
        {
            if (!string.IsNullOrEmpty(PayId))
            {
                creditCardService.Cancel(PayId);
                PayId = null;
            }
        }

        private void AddCanceledEvents()
        {
            _events.Add(new OrderCanceledEvent { OrderId = OrderId, ProductName = ProductName, Price = Price, PayId = PayId, LastUserId = LastUserId });            
        }

        private void AddCompletedEvents()
        {
            _events.Add(new OrderCompletedOutTranEvent { OrderId = OrderId });
            _events.Add(new OrderCompletedExceptionEvent { OrderId = OrderId });            
        }
    }

    public class OrderCanceledModel : IAggregateRoot
    {
        public static OrderCanceledModel NewCanceled(int orderId)
        {
            OrderCanceledModel orderCanceledModel = new OrderCanceledModel();
            orderCanceledModel.OrderId = orderId;

            return orderCanceledModel;
        }


        protected OrderCanceledModel()
        {
            Dirty = Dirty.False();
            Deleted = Deleted.False();

            DomainModelMarker.Mark(this);
        }

        public int OrderId { get; private set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; set; }

        public string LastUserId { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public Deleted Deleted { get; private set; }

        private List<IDomainEvent> _events = new List<IDomainEvent>();

        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public void Save()
        {
            Dirty = Dirty.True();
        }        
    }

    public class OrderCanceledEvent : IDomainEvent
    {
        public int OrderId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; set; }

        public string LastUserId { get; set; }
    }

    public class OrderCompletedOutTranEvent : IDomainOutTranEvent
    {           
        public bool Async { get; set; }
        public int OrderId { get; set; }
    }

    public class OrderCompletedExceptionEvent : IDomainExceptionEvent
    {   
        public Exception Exception { get; set; }
        public int OrderId { get; set; }
    }
}