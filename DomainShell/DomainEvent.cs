using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEvent
    {
        
    }

    public interface IDomainOutTranEvent : IDomainEvent
    {
        bool Async { get; set; }
    }

    public interface IDomainExceptionEvent : IDomainEvent
    {
        Exception Exception { get; set; }
    }

    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }


    public interface IDomainEventScope : IDisposable
    {
        IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;        
    }

    public interface IDomainEventAuthor
    {
        IEnumerable<IDomainEvent> GetEvents();
        void ClearEvents();
    }

    public interface IDomainEventList
    {
        void Add(IDomainEvent[] domainEvents);
        void Add(IDomainEventAuthor domainEventAuthor);
        IEnumerable<IDomainEvent> GetInTranEvents();
        IEnumerable<IDomainEvent> GetOutTranEvents();
        IEnumerable<IDomainEvent> GetOutTranAsyncEvents();
        IEnumerable<IDomainEvent> GetExceptionEvents();
        void Remove(params IDomainEvent[] domainEvents);
        void Clear();
    }

    public interface IDomainEventPublisher
    {        
        void PublishInTran();
        void PublishOutTran();
        void PublishByException(Exception exception);
        void Revoke();
    }
}
