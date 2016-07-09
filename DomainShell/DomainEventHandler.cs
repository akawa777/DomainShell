using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Base;
using DomainShell.Infrastructure;

namespace DomainShell
{  
    public abstract class DomainEvent : IDomainEvent
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public abstract class DomainEvent<TResult> : IDomainEvent
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public interface IDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : DomainEvent
    {        
        void Handle(TEvent @event);
    }

    public interface IDomainEventHandler<TEvent, TResult> : IDomainEventHandler where TEvent : DomainEvent<TResult>
    {        
        TResult Handle(TEvent @event);
    }    
}
