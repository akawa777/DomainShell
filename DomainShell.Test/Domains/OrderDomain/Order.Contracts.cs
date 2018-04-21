using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains.OrderDomain
{    
    public interface IOrderRepository
    {
        Order Find(int orderId, bool throwError = false); 
        void Save(Order order);
    }

    public interface IOrderReadRepository
    {
        OrderRead GetLastByUser(string userId);
    }

    public class PaymentId
    {
        public PaymentId(string paymenId)
        {
            Value = paymenId;
        }

        public string Value { get; }
    }

    public interface IOrderService
    {
        PaymentId Pay(Order order);
        bool ExistsUser(Order order);
    }
}