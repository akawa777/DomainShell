using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;
using System.Collections.Specialized;
using DomainShell.Domain;

namespace DomainShell.Test
{
    public interface IConnection : IDisposable
    {
        IDbCommand CreateCommand();
    }

    public interface IDomainEvent
    {

    }

    public interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }

    public interface IDomainEventAsyncHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }

    public interface IDomainEventExceptionHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent, Exception exception);
    }

    public interface IAggregateRoot
    {
        IDomainEvent[] GetDomainEvents();

        void ClearDomainEvents();

        ModelState ModelState { get; }

        bool Deleted { get; }

        string LastUpdate { get; }
    }

    public abstract class AggregateRoot : IAggregateRoot
    {
        protected List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public IDomainEvent[] GetDomainEvents()
        {
            return DomainEvents.ToArray();
        }

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }

        public bool Deleted { get; protected set; }

        public ModelState ModelState { get; protected set; }

        public string LastUpdate { get; private set; }
    }
}