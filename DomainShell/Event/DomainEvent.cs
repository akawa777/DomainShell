using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;

namespace DomainShell.Event
{
    public class EventResult : MessageResult
    {
        public void Set<TDomainEvent, TResult>(TDomainEvent domainEvent, TResult result) where TDomainEvent : IDomainEvent<TResult>
        {
            (this as IMessageResult).Set(domainEvent, result);
        }
    }

    public interface IDomainEvent : IMessage
    {
        
    }

    public interface ITransaction
    {
        bool InTransaction();
    }

    public interface IDomainEvent<TResult> : IDomainEvent, IMessage<TResult>
    {

    }

    public interface IDomainEventHandler : IMessageHandler
    {
        EventResult EventResult { get; }
    }

    public interface IDomainEventHandler<TDomainEvent> : IDomainEventHandler, IMessageHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }

    internal interface IDomainEventCallbackCache
    {
        Dictionary<IDomainEvent, dynamic> GetCallbackCache();
    }
}
