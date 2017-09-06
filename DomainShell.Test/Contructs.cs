using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test
{
    public interface IConnection : IDisposable
    {
        IDbCommand CreateCommand();
    }

    public interface IAggregateRoot : IDomainEventAuthor
    {
        string LastUserId { get; set; }

        int RecordVersion { get; }

        Dirty Dirty { get; }

        bool Deleted { get; }
    }

    public enum ModelState
    {
        Added,
        Deleted,
        Modified,
        Unchanged
    }
    
    public interface IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
       void Save(TAggregateRoot aggregate);
    }

    public interface IOrderRepository
    {
        OrderModel Find(int orderId, bool throwError = false);
        OrderModel GetLastByUser(string userId);        
    }

    public interface IOrderCanceledRepository
    {
        OrderCanceledModel Find(int orderId, bool throwError = false);
    }

    public interface IOrderValidator
    {
        void ValidateWhenRegister(OrderModel orderModel);
        void ValidateWhenComplete(OrderModel orderModel);
        void ValidateWhenCancel(OrderModel orderModel);
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