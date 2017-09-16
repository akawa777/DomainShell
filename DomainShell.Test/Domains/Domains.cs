using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class UserModel : IAggregateRoot
    {
        protected UserModel()
        {
            
        }

        public string UserId { get; private set; }

        public string UserName { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public bool Deleted { get; private set; }
    }

    public class UserValue
    {
        protected UserValue()
        {

        }

        public static UserValue Create(UserModel userModel)
        {
            if (userModel == null || userModel.RecordVersion == 0 || userModel.Dirty.Is()) throw new ArgumentException("userModel is invalid.");

            UserValue userValue = new UserValue();
            userValue.UserId = userModel.UserId;

            return userValue;
        }

        public string UserId { get; private set; }
    }

    public class OrderModel : IAggregateRoot, IDomainEventAuthor
    {
        public static OrderModel NewOrder()
        {
            OrderModel orderModel = DomainModelProxyFactory.Create<OrderModel>();
            
            return orderModel;
        }

        protected OrderModel()
        {
            
        }

        public int OrderId { get; private set; }               

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string CreditCardCode { get; private set; }

        public string PayId { get; private set; }

        public UserValue LastUser { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

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

        public void Register(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenRegister(this);

            Dirty = Dirty.Seal(this);
        }

        public void Cancel(IOrderValidator orderValidator)
        {
            orderValidator.ValidateWhenCancel(this);            

            Deleted = true;

            AddCanceledEvents();

            Dirty = Dirty.Seal(this);
        }

        public virtual void Complete(IOrderValidator orderValidator, ICreditCardService creditCardService, string creditCardCode)
        {
            orderValidator.ValidateWhenComplete(this, creditCardCode);

            CreditCardCode = creditCardCode;            

            Pay(creditCardService);

            AddCompletedEvents();

            Dirty = Dirty.Seal(this);
        }

        public void SendCompletedMail(IMailService mailService)
        {
            mailService.Send("xxx", "xxx", "xxx");
        }

        public void CancelCompleted(ICreditCardService creditCardService)
        {
            CancelPay(creditCardService);

            Dirty = Dirty.Seal(this);
        }

        private void Pay(ICreditCardService creditCardService)
        {
            PayId = creditCardService.Pay(CreditCardCode, Price);
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
            _events.Add(new OrderCanceledEvent { OrderId = OrderId, ProductName = ProductName, Price = Price, PayId = PayId, LastUser = LastUser });            
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
            
        }

        public int OrderId { get; private set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string CreditCardCode { get; private set; }

        public string PayId { get; set; }

        public UserValue LastUser { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public bool Deleted { get; private set; }

        public void Register()
        {
            Dirty = Dirty.Seal(this);
        }        
    }

    public class OrderCanceledEvent : IDomainEvent
    {
        public int OrderId { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string PayId { get; set; }

        public UserValue LastUser { get; set; }
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

    public class OrderSummaryValue
    {
        protected OrderSummaryValue()
        {

        }

        public string ProductName { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal TotalOrderNo { get; private set; }
    }
}