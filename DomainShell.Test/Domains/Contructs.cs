using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains
{
    public interface IConnection
    {
        IDbCommand CreateCommand();
    }

    public interface IAggregateRoot
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

    public interface IMonthlyOrderRepository
    {        
        MonthlyOrderModel GetMonthlyByUserId(string userId, DateTime orderDate, int excludeOrderId = 0);        
    }

    public interface ICreditCardService
    {
        string Pay(OrderModel orderModel);
        void Cancel(OrderModel orderModel);
    }

    public interface IMailService
    {
        void Send(OrderModel orderModel);
    }

    public interface IOrderBudgetCheckService
    {
        bool IsOverBudget(OrderModel orderModel);
    }
}