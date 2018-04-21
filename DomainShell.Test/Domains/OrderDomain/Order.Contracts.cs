using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains.OrderDomain
{    
    public interface IOrderRepository
    {
        Order Find(int orderId);         
        Order GetLastByUser(string userId);        
        void Save(Order order);
    }

    public interface IOrderReadRepository
    {
        OrderRead Find(int orderId);
    }

    public class PaymentResult
    {
        public PaymentResult(string paymenId, int paymentPoint)
        {
            PaymentId = paymenId;
            PaymentPoint = paymentPoint;
        }

        public string PaymentId { get; }
        public int PaymentPoint { get; }
    }

    public interface IOrderService
    {
        PaymentResult Pay(Order order);
        void CancelPayment(Order order);
        bool ExistsUser(Order order);
    }
}