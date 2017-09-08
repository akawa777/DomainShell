using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains
{
    public interface ICurrentConnection
    {
        IDbCommand CreateCommand();
    }

    public interface IAggregateRoot : IDomainEventAuthor
    {
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

    public interface IUserRepository
    {
        UserModel Find(string userId, bool throwError = false);
    }

    public interface IOrderRepository : IWriteRepository<OrderModel>
    {
        OrderModel Find(int orderId, bool throwError = false);
        OrderModel GetLastByUser(string userId);        
    }

    public interface IOrderCanceledRepository : IWriteRepository<OrderCanceledModel> 
    {
        OrderCanceledModel Find(int orderId, bool throwError = false);
    }

    public interface IOrderSummaryReader
    {
        IEnumerable<OrderSummaryValue> GetSummary();
    }

    public interface IOrderValidator
    {
        void ValidateWhenRegister(OrderModel orderModel);
        void ValidateWhenComplete(OrderModel orderModel, string creditCardCord);
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