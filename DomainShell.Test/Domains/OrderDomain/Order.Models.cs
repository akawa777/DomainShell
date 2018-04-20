using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains.UserDomain;

namespace DomainShell.Test.Domains.OrderDomain
{
    public class Order : IAggregateRoot, IDomainEventAuthor
    {
        public static Order NewOrder()
        {
            Order order = DomainModelFactory.Create<Order>();
            
            return order;
        }

        protected Order()
        {
            DomainModelTracker.Mark(this);
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
        
        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            return _events;
        }

        public void ClearDomainEvents()
        {
            _events.Clear();
        }

        public void Register(IOrderService orderService)
        {
            ValidateWhenRegister(orderService);

            Dirty = Dirty.Seal(this);
        }

        public void Cancel()
        {
            ValidateWhenCancel();

            Deleted = true;

            AddCanceledEvents();

            Dirty = Dirty.Seal(this);
        }

        public virtual void Complete(IOrderService orderService, string creditCardCode)
        {
            CreditCardCode = creditCardCode;            

            ValidateWhenComplete();            

            PayId = orderService.Pay(this);

            AddCompletedEvents();

            Dirty = Dirty.Seal(this);
        }

        public void SendCompletedMail(IOrderService orderService)
        {
            orderService.SendMail(this);

            System.Threading.Thread.Sleep(5000);
        }

        public void SendCompleteMail()
        {
            AddMailSendedEvents();

            System.Threading.Thread.Sleep(5000);
        }

        public void CancelCompleted(IOrderService orderService)
        {
            ValidateWhenCancelCompleted();

            orderService.Cancel(this);

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

        private void AddMailSendedEvents()
        {
            _events.Add(new OrderSendedMailEvent { OrderId = OrderId });
        }

        private void ValidateWhenRegister(IOrderService orderServicee)
        {            
            if (string.IsNullOrEmpty(ProductName)) throw new Exception("ProductName is required.");
            if (User == null) throw new Exception("User is required.");
            if (OrderDate == null) throw new Exception("OrderDate is required.");
            if (orderServicee.IsOverBudget(this)) throw new Exception("BudgetAmount is over.");
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
            OrderCanceledModel orderCanceledModel = new OrderCanceledModel
            {
                OrderId = orderId
            };

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
        public DomainEventMode Mode => DomainEventMode.InSession();

        public int OrderId { get; set; }
        public UserValue User { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string PayId { get; set; }        
    }

    public class OrderCompletedEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.OutSession();

        public int OrderId { get; set; }
    }

    public class OrderCompletedExceptionEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.AtException();

        public int OrderId { get; set; }
    }

    public class OrderSendedMailEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.InSession();

        public int OrderId { get; set; }
    }

    public class MonthlyOrder
    {
        protected MonthlyOrder()
        {
            DomainModelTracker.Mark(this);
        }

        public string UserId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public decimal Budget { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal TotalOrderNo { get; private set; }
        public bool IsOverBudgetByIncludingPrice(decimal price) => Budget < TotalPrice + price;
    }

    public class SpecialOrder : Order
    {
        public static SpecialOrder NewSpecialOrder()
        {
            SpecialOrder order = DomainModelFactory.Create<SpecialOrder>();

            return order;
        }

        protected SpecialOrder()
        {
            DomainModelTracker.Mark(this);
        }

        public override void Complete(IOrderService orderService, string creditCardCode)
        {
            OutputLog($"{nameof(SpecialOrder)} {nameof(Complete)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            base.Complete(orderService, creditCardCode);
        }

        public Type GetImplementType()
        {
            return typeof(Order);
        }

        private void OutputLog(string message)
        {
            Console.WriteLine($"logging {message}");
        }
    }
}