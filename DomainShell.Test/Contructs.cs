using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public interface IOrderRepository
    {
        OrderModel Find(string orderId, bool throwError = false);
        void Apply(OrderModel orderModel);
    }

    public interface IOrderValidator
    {
        void ValidateWhenRegist(OrderModel orderModel);
        void ValidateWhenComplete(OrderModel orderModel);
    }

    public interface ICreditCardService
    {
        string Pay(string creditCardCord, decimal price);
        void Cancel(string payId);
    }

    public interface IMailService
    {
        void Send(string emailAddress, string title, string contents);
    }
}