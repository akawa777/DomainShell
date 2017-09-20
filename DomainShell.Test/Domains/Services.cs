using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class OrderValidator : IOrderValidator
    {   
        public OrderValidator(IOrderSummaryReader orderSummaryReader)
        {
            _orderSummaryReader = orderSummaryReader;
        }

        private IOrderSummaryReader _orderSummaryReader;        

        public void ValidateWhenRegister(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenRegister)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (string.IsNullOrEmpty(orderModel.ProductName)) throw new Exception("ProductName is required.");
            if (orderModel.User == null) throw new Exception("User is required.");
            if (IsOverBudgetAmount(orderModel)) throw new Exception("BudgetAmount is over.");
        }

        private bool IsOverBudgetAmount(OrderModel orderModel)
        {   
            OrderSummaryValue orderSummaryValue = _orderSummaryReader.GetSummaryByUserId(orderModel.User.UserId);

            return orderSummaryValue.IsOverBuget;
        }

        public void ValidateWhenComplete(OrderModel orderModel, string creditCardCode)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenComplete)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            ValidateWhenRegister(orderModel);
            if (!string.IsNullOrEmpty(orderModel.PayId)) throw new Exception("already paid.");
            if (string.IsNullOrEmpty(creditCardCode)) throw new Exception("creditCardCode is required.");            
        }

        public void ValidateWhenCancel(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenCancel)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (!string.IsNullOrEmpty(orderModel.PayId)) throw new Exception("already paid.");            
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