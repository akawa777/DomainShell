using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public class OrderValidator : IOrderValidator
    {
        public void ValidateWhenRegist(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenRegist)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        public void ValidateWhenComplete(OrderModel orderModel)
        {
            Console.WriteLine($"{nameof(OrderValidator)} {nameof(ValidateWhenRegist)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }
    }

    public class CreditCardService : ICreditCardService
    {
        public void Pay(string creditCardCord, decimal price, out string payId)
        {
            payId = Guid.NewGuid().ToString();
            Console.WriteLine($"{nameof(CreditCardService)} {nameof(Pay)} {System.Threading.Thread.CurrentThread.ManagedThreadId}");
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