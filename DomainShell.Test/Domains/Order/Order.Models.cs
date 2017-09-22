using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains.User;

namespace DomainShell.Test.Domains.Order
{
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

        public UserValue User { get; set; }

        public DateTime? OrderDate { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string CreditCardCode { get; private set; }

        public string PayId { get; private set; }        

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

        public void Register(IOrderBudgetCheckService orderBudgetCheckService)
        {
            ValidateWhenRegister(orderBudgetCheckService);

            Dirty = Dirty.Seal(this);
        }

        public void Cancel()
        {
            ValidateWhenCancel();

            Deleted = true;

            AddCanceledEvents();

            Dirty = Dirty.Seal(this);
        }

        public virtual void Complete(ICreditCardService creditCardService, string creditCardCode)
        {
            CreditCardCode = creditCardCode;            

            ValidateWhenComplete();            

            creditCardService.Pay(this);

            AddCompletedEvents();

            Dirty = Dirty.Seal(this);
        }

        public void SendCompletedMail(IMailService mailService)
        {
            mailService.Send(this);
        }

        public void CancelCompleted(ICreditCardService creditCardService)
        {
            ValidateWhenCancelCompleted();

            creditCardService.Cancel(this);

            Dirty = Dirty.Seal(this);
        }
        
        private void AddCanceledEvents()
        {
            _events.Add(new OrderCanceledEvent { OrderId = OrderId, ProductName = ProductName, Price = Price, PayId = PayId, User = User });            
        }

        private void AddCompletedEvents()
        {
            _events.Add(new OrderCompletedEvent { OrderId = OrderId });
            _events.Add(new OrderCompletedExceptionEvent { OrderId = OrderId });            
        }

        private void ValidateWhenRegister(IOrderBudgetCheckService orderBudgetCheckService)
        {            
            if (string.IsNullOrEmpty(ProductName)) throw new Exception("ProductName is required.");
            if (User == null) throw new Exception("User is required.");
            if (OrderDate == null) throw new Exception("OrderDate is required.");
            if (orderBudgetCheckService.IsOverBudget(this)) throw new Exception("BudgetAmount is over.");
        }

        private void ValidateWhenComplete()
        {            
            if (!string.IsNullOrEmpty(PayId)) throw new Exception("already paid.");
            if (string.IsNullOrEmpty(CreditCardCode)) throw new Exception("creditCardCode is required.");            
        }

        private void ValidateWhenCancel()
        {
            if (!string.IsNullOrEmpty(PayId)) throw new Exception("already paid.");            
        }

        private void ValidateWhenCancelCompleted()
        {
            if (string.IsNullOrEmpty(PayId)) throw new Exception("payment is not completed.");            
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

        public UserValue User { get; set; }

        public DateTime? OrderDate { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string CreditCardCode { get; set; }

        public string PayId { get; set; }

        public int RecordVersion { get; private set; }

        public Dirty Dirty { get; private set; }

        public bool Deleted { get; private set; }

        public void Register()
        {
            ValidateWhenRegister();
            Dirty = Dirty.Seal(this);
        }

        private void ValidateWhenRegister()
        {
            if (OrderId <= 0) throw new Exception("OrderId is invalid.");
            if (string.IsNullOrEmpty(ProductName)) throw new Exception("ProductName is required.");
            if (User == null) throw new Exception("User is required.");
            if (OrderDate == null) throw new Exception("OrderDate is required.");            
        }
    }

    public class OrderCanceledEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.InTran();

        public int OrderId { get; set; }
        public UserValue User { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string PayId { get; set; }        
    }

    public class OrderCompletedEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.OutTran();

        public int OrderId { get; set; }
    }

    public class OrderCompletedExceptionEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.AtException();

        public int OrderId { get; set; }
    }

    public class MonthlyOrderModel
    {
        protected MonthlyOrderModel()
        {

        }

        public string UserId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal Budget { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal TotalOrderNo { get; private set; }
        public bool IsOverBudgetByIncludingPrice(decimal price) => Budget < TotalPrice + price;
    }
}