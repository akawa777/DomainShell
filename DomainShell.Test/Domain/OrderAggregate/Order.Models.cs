using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DomainShell.Domain;

namespace DomainShell.Test.Domain.OrderAggregate
{
    public class Order : AggregateRoot
    {
        public static Order Create(bool isSpecialOrder)
        {
            Order order;

            if (!isSpecialOrder)
            {
                order = new Order();
            }
            else
            {
                order = SpecialOrder.Create();
            }
            
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

        public void IncrementCertificateIssueCount(IOrderService orderService)
        {
            ValidatePrecondition(orderService);

            if (string.IsNullOrEmpty(PaymentId)) throw new Exception("order has not been paid.");

            CertificateIssueCount++;

            ModelState = ModelState.Seal(this);
        }

        public void Pay(IOrderService orderService, string creditCardCode)
        {
            ValidatePrecondition(orderService);

            if (!string.IsNullOrEmpty(PaymentId)) throw new Exception("order has already been paid.");
            if (string.IsNullOrEmpty(creditCardCode)) throw new Exception("creditCardCode is required.");            

            CreditCardCode = creditCardCode;    

            var paymentResult = orderService.Pay(this);

            PaymentId = paymentResult.PaymentId; 

            DomainEvents.Add(new OrderPaidEvent{ OrderId = OrderId, UserId = UserId, PaymentPoint = paymentResult.PaymentPoint });

            ModelState = ModelState.Seal(this);
        }

        public void CancelPayment(IOrderService orderService)
        {
            ValidatePrecondition(orderService);

            if (string.IsNullOrEmpty(PaymentId)) throw new Exception("order has not been paid.");
            if (string.IsNullOrEmpty(CreditCardCode)) throw new Exception("creditCardCode is required.");            

            orderService.CancelPayment(this);

            this.CreditCardCode = null;
            this.PaymentId = null;

            ModelState = ModelState.Seal(this);
        }
        
        private void ValidatePrecondition(IOrderService orderService)
        {            
            if (string.IsNullOrEmpty(ProductName)) throw new Exception("ProductName is required.");
            if (string.IsNullOrEmpty(UserId))throw new Exception("UserId is required.");
            if (!string.IsNullOrEmpty(UserId) && !orderService.ExistsUser(this)) throw new Exception("UserId is invalid.");
            if (OrderDate == null) throw new Exception("OrderDate is required.");
        }
    }

    public class SpecialOrder : Order
    {
        public static SpecialOrder Create()
        {
            return new SpecialOrder();
        }

        protected SpecialOrder()
        {
            
        }
    }

    public class OrderPaidEvent : IDomainEvent
    {
        public string UserId { get; set; }
        public int OrderId { get; set; }
        public int PaymentPoint { get; set; }
    }

    public class OrderRead
    {
        public static OrderRead Create()
        {
            return new OrderRead();
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
    }
}