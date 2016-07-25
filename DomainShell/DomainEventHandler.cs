using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Base;

namespace DomainShell
{
    public interface IDomainEvent
    {
        IAggregateRoot AggregateRoot { get; set; }
    }

    public interface IDomainEvent<TResult> : IDomainEvent
    {
        
    }

    public interface IDomainEventHandler
    {
        
    }

    //public abstract class DomainEvent : IDomainEvent
    //{
    //    public IAggregateRoot AggregateRoot { get; set; }
    //}

    //public abstract class DomainEvent<TResult> : IDomainEvent
    //{
    //    public IAggregateRoot AggregateRoot { get; set; }
    //}

    public interface IDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : IDomainEvent
    {        
        void Handle(TEvent @event);
    }

    public interface IDomainEventHandler<TEvent, TResult> : IDomainEventHandler where TEvent : IDomainEvent<TResult>
    {        
        TResult Handle(TEvent @event);
    }    
}
