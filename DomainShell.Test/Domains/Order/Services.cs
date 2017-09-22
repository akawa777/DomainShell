using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains.Order
{
    public class OrderBudgetCheckService : IOrderBudgetCheckService
    {   
        public OrderBudgetCheckService(IMonthlyOrderRepository monthlyOrderRepository)
        {
            _monthlyOrderRepository = monthlyOrderRepository;
        }

        private IMonthlyOrderRepository _monthlyOrderRepository; 

        public bool IsOverBudget(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderBudgetCheckService)} {nameof(IsOverBudget)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            
            MonthlyOrderModel monthlyOrderModel = _monthlyOrderRepository.GetMonthlyByUserId(orderModel.User.UserId, orderModel.OrderDate.Value, excludeOrderId: orderModel.OrderId);

            return monthlyOrderModel.IsOverBudgetByIncludingPrice(orderModel.Price);
        }
    }

    public class CreditCardService : ICreditCardService
    {
        public string Pay(OrderModel orderModel)
        {            
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Pay)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            string payId = Guid.NewGuid().ToString();

            return payId;
        }

        public void Cancel(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Cancel)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class MailService : IMailService
    {
        public void Send(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(MailService)} {nameof(Send)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }
}