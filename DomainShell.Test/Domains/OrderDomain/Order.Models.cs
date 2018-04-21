using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using DomainShell.Test.Domains.UserDomain;

namespace DomainShell.Test.Domains.OrderDomain
{
    public class Order : AggregateRoot
    {
        public static Order NewOrder()
        {
            var order = DomainModelFactory.Create<Order>();
            
            return order;
        }

        protected Order() : base()
        {
            
        }

        public int OrderId { get; private set; }

        public string UserId { get; set; }

        public DateTime? OrderDate { get; set; }

        public string ProductName { get; set; }

        public decimal Price { get; set; }

        public string CreditCardCode { get; private set; }

        public string PaymentId { get; private set; }      

        public int CertificateIssueCount { get; private set; }                  

        public void Register(IOrderService orderService)
        {
            ValidatePrecondition(orderService);

            DomainEvents.Add(new OrderRegisterdEvent{ OrderId = OrderId });

            State = ModelState.Seal(this);
        }

        public virtual void Pay(IOrderService orderService, string creditCardCode)
        {
            ValidatePrecondition(orderService);

            if (!string.IsNullOrEmpty(PaymentId)) throw new Exception("already paid.");
            if (string.IsNullOrEmpty(creditCardCode)) throw new Exception("creditCardCode is required.");            

            CreditCardCode = creditCardCode;    

            var paymentId = orderService.Pay(this);

            PaymentId = paymentId.Value; 

            DomainEvents.Add(new OrderPaidExceptionEvent { PaymentId = PaymentId });

            State = ModelState.Seal(this);
        }

        public System.IO.Stream IssueCertificate()
        {
            if (string.IsNullOrEmpty(PaymentId)) throw new Exception("not paid.");

            DomainEvents.Add(new OrderIssuedCertificateEvent{ OrderId = OrderId });

            return new System.IO.MemoryStream();
        }
        
        private void ValidatePrecondition(IOrderService orderService)
        {            
            if (string.IsNullOrEmpty(ProductName)) throw new Exception("ProductName is required.");
            if (string.IsNullOrEmpty(UserId))throw new Exception("UserId is required.");
            if (!string.IsNullOrEmpty(UserId) && !orderService.ExistsUser(this)) throw new Exception("UserId is invalid.");
            if (OrderDate == null) throw new Exception("OrderDate is required.");
        }
    }

    public class OrderRegisterdEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.InSession();

        public int OrderId { get; set; }
    }

    public class OrderPaidExceptionEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.AtException();

        public string PaymentId { get; set; }
    }

    public class OrderIssuedCertificateEvent : IDomainEvent
    {
        public DomainEventMode Mode => DomainEventMode.OutSession();

        public int OrderId { get; set; }
    }

    public class OrderRead : ReadAggregateRoot
    {
        public static OrderRead Create()
        {
            OrderRead orderRead = DomainModelFactory.Create<OrderRead>();
            
            return orderRead;
        }

        protected OrderRead() : base()
        {
            
        }

        public int OrderId { get; private set; }

        public string ProductName { get; private set; }

        public decimal Price { get; private set; }
    }
}