using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains.OrderDomain
{    
    public class OrderService : IOrderService
    {
        public OrderService(IMonthlyOrderRepository monthlyOrderRepository)
        {
            _monthlyOrderRepository = monthlyOrderRepository;
        }

        private IMonthlyOrderRepository _monthlyOrderRepository;

        public string Pay(Order order)
        {            
            Console.WriteLine($"{nameof(OrderService)} {nameof(Pay)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            string payId = Guid.NewGuid().ToString();

            return payId;
        }

        public void Cancel(Order order)
        {
            Console.WriteLine($"{nameof(OrderService)} {nameof(Cancel)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public virtual void SendMail(Order order)
        {
            SendMailBy(order as dynamic);
        }

        private void SendMailBy(Order order)
        {
            Console.WriteLine($"{nameof(OrderService)} {nameof(SendMailBy)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        private void SendMailBy(SpecialOrder order)
        {
            Console.WriteLine($"{nameof(OrderService)} {nameof(SendMailBy)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public bool IsOverBudget(Order order)
        {
            Console.WriteLine($"{nameof(OrderService)} {nameof(IsOverBudget)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            MonthlyOrder monthlyOrder = _monthlyOrderRepository.GetMonthlyByUserId(order.User.UserId, order.OrderDate.Value, excludeOrderId: order.OrderId);

            return monthlyOrder.IsOverBudgetByIncludingPrice(order.Price);
        }
    }
}