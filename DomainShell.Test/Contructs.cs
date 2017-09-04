using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;

namespace DomainShell.Test
{
    public interface IAggregateRoot : IDomainEventAuthor
    {
        string LastUserId { get; set; }

        int RecordVersion { get; }

        bool Dirty { get; }

        bool Deleted { get; }
    }

    public enum ModelState
    {
        Added,
        Deleted,
        Modified,
        Unchanged
    }

    //public interface IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    //{
    //    TAggregateRoot GetStored(TAggregateRoot aggregate);
    //    void Insert(TAggregateRoot aggregate);
    //    void Update(TAggregateRoot aggregate);
    //    void Delete(TAggregateRoot aggregate);
    //}

    public interface IOrderRepository
    {
        OrderModel Find(string orderId, bool throwError = false);
        OrderModel GetLastByUser(string userId);
        void Save(OrderModel orderModel);
    }

    public interface IOrderValidator
    {
        void ValidateWhenRegister(OrderModel orderModel);
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