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
            OrderModel orderModel = new OrderModel();            
            
            return orderModel;
        }

        protected OrderModel()
        {
            Dirty = Dirty.False();
            Deleted = Deleted.False();

            DomainModelMarker.Mark(this);
        }

        public string OrderId { get; private set; }               

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

        public void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
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

        private void AddCompletedEvents()
        {
            _events.Add(new OrderCompletedEvent { OrderId = OrderId });
            _events.Add(new OrderCompletedExceptionEvent { OrderId = OrderId });            
        }
    }

    public class OrderCompletedEvent : IDomainOutTranEvent
    {           
        public bool Async { get; set; }
        public string OrderId { get; set; }
    }

    public class OrderCompletedExceptionEvent : IDomainExceptionEvent
    {   
        public Exception Exception { get; set; }
        public string OrderId { get; set; }
    }
}