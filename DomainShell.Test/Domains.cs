using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderModel : IDomainEventAuthor
    {
        public static OrderModel NewOrder()
        {
            OrderModel orderModel = new OrderModel();            
            orderModel.OrderId = Guid.NewGuid().ToString();
            orderModel.Dirty = true;
            return orderModel;
        }

        protected OrderModel()
        {
        }

        public string OrderId { get; private set; }       

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; private set; }

        public int RecordVersion { get; private set; }

        public bool Dirty { get; private set; }

        public bool Deleted { get; private set; }

        private List<IDomainEvent> _events = new List<IDomainEvent>();
        
        public IEnumerable<IDomainEvent> GetEvents()
        {
            return _events;
        }

        public void ClearEvents()
        {
            _events.Clear();
        }

        public void Regist(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenRegist(this);

            Dirty = true;
        }

        public void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
        {
            orderValidator.ValidateWhenComplete(this);

            Dirty = true;
            creditCardService.Pay(creditCardCode, Price, out string payId);
            PayId = payId;

            _events.Add(new OrderCompletedEvent{ Async = false, OrderId = OrderId });
            _events.Add(new OrderCompletedExceptionEvent{ OrderId = OrderId });
        }

        public void SendCompletedMail(IMailService mailService)
        {
            mailService.Send("x", "x", "x");
        }

        public void CancelCompleted(ICreditCardService creditCardService)
        {
            Dirty = true;

            if (!string.IsNullOrEmpty(PayId))
            {
                creditCardService.Cancel(PayId);
                PayId = null;
            }
        }
    }

    public class OrderCompletedEvent : IDomainOuterTranEvent
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