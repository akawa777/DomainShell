using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
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

        protected Order()
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

        public virtual void Register(IOrderService orderService)
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
        public DomainEventMode Mode { get; } = DomainEventMode.InSession();

        public int OrderId { get; set; }
    }

    public class OrderPaidExceptionEvent : IDomainEvent
    {
        public DomainEventMode Mode { get; } = DomainEventMode.OnException();

        public string PaymentId { get; set; }
    }

    public class OrderRead : ReadAggregateRoot
    {
        public static OrderRead Create()
        {
            OrderRead orderRead = DomainModelFactory.Create<OrderRead>();
            
            return orderRead;
        }

        protected OrderRead()
        {
            
        }

        public int OrderId { get; private set; }

        public string UserId { get; private set; }

        public DateTime? OrderDate { get; private set; }

        public string ProductName { get; private set; }

        public decimal Price { get; private set; }

        public string PaymentId { get; private set; } 

        public virtual Stream IssueCertificate()
        {
            if (string.IsNullOrEmpty(PaymentId)) throw new Exception("not paid.");

            DomainEvents.Add(new OrderReadIssuedCertificateEvent{ OrderId = OrderId });

            using (var memory = new MemoryStream())            
            using (var writer = new StreamWriter(memory))
            {
                writer.Write($@"
                    {nameof(OrderId)}: {OrderId},
                    {nameof(UserId)}: {UserId},
                    {nameof(OrderDate)}: {OrderDate},
                    {nameof(ProductName)}: {ProductName},
                    {nameof(Price)}: {Price}
                ");

                memory.Position = 0;

                return memory;
            }
        }        
    }

    public class OrderReadIssuedCertificateEvent : IDomainEvent
    {
        public DomainEventMode Mode { get; } = DomainEventMode.OutSession();

        public int OrderId { get; set; }
    }
}