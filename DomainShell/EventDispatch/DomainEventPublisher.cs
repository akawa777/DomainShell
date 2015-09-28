using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;
using DomainShell.Event;

namespace DomainShell.EventDispatch
{   
    public interface IDomainEventPublisher
    {
        void Publish<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent;
        void Callback<TResult>(IDomainEvent<TResult> message, Action<TResult> action);
    }

    public class DomainEventPublisher : IDomainEventPublisher
    {
        private MessagePublisher _messagePublisher = new MessagePublisher();

        public void Register<TDomainEvent>(Func<IDomainEventHandler<TDomainEvent>> handler) where TDomainEvent : IDomainEvent
        {
            _messagePublisher.Register(handler);
        }

        public void Callback<TResult>(IDomainEvent<TResult> domainEvent, Action<TResult> action)
        {
            _messagePublisher.Callback(domainEvent, action);
        }

        public void Publish<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            _messagePublisher.Publish(domainEvent, handler => (handler as IDomainEventHandler).EventResult);
        }

        public void SetBeginHandle(Action<IDomainEvent, IDomainEventHandler> beginHandle)
        {
            _messagePublisher.SetBeginHandle((@event, handler) => beginHandle(@event as IDomainEvent, handler as IDomainEventHandler));
        }

        public void SetEndHandle(Action<IDomainEvent, IDomainEventHandler, Exception> endHandle)
        {
            _messagePublisher.SetEndHandle((@event, handler, exception) => endHandle(@event as IDomainEvent, handler as IDomainEventHandler, exception));
        }
    }   
}
