using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class OrderBudgetCheckService : IOrderBudgetCheckService
    {   
        public OrderBudgetCheckService(IOrderSummaryRepository orderSummaryRepository)
        {
            _orderSummaryRepository = orderSummaryRepository;
        }

        private IOrderSummaryRepository _orderSummaryRepository; 

        public bool IsOverBudget(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderBudgetCheckService)} {nameof(IsOverBudget)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            OrderSummaryModel orderSummaryModel = _orderSummaryRepository.GetByUserId(orderModel.User.UserId, excludeOrderId: orderModel.OrderId);

            orderSummaryModel.IncreaseTotalPrice(orderModel.Price);

            return orderSummaryModel.IsOverBudget;
        }
    }

    public class CreditCardService : ICreditCardService
    {
        public string Pay(string creditCardCord, decimal price)
        {            
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Pay)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            string payId = Guid.NewGuid().ToString();

            return payId;
        }

        public void Cancel(string payId)
        {
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Cancel)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class MailService : IMailService
    {
        public void Send(string emailAddress, string title, string contents)
        {
            Console.WriteLine($"{nameof(MailService)} {nameof(Send)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }
}