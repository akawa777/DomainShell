using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test.Domains
{
    public class OrderValidator : IOrderValidator
    {
        public void ValidateWhenRegister(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenRegister)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public void ValidateWhenComplete(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenComplete)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public void ValidateWhenCancel(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenCancel)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
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