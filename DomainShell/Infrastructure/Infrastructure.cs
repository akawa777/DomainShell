using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Base;

namespace DomainShell.Infrastructure
{
    public interface IDomainEventRegister
    {
        void Set(Type eventType, Func<object> handler);
        void Set<TEvent>(Func<IDomainEventHandler> handler) where TEvent : IDomainEvent;
    }

    public interface IDomainEventLoader
    {
        IDomainEventHandler Load(IDomainEvent @event);
    }

    internal class DomainEventContainer : IDomainEventRegister, IDomainEventLoader
    {
        private Dictionary<Type, Func<IDomainEventHandler>> _handlerMap = new Dictionary<Type, Func<IDomainEventHandler>>();

        public void Set(Type eventType, Func<object> handler)
        {            
            _handlerMap[eventType] = () =>
            {
                return handler() as IDomainEventHandler;
            };
        }

        public void Set<TEvent>(Func<IDomainEventHandler> handler) where TEvent : IDomainEvent
        {
            _handlerMap[typeof(TEvent)] = () =>
            {
                return handler();
            };
        }

        public IDomainEventHandler Load(IDomainEvent @event)
        {
            if (!_handlerMap.ContainsKey(@event.GetType()))
            {
                return null;
            }

            Func<IDomainEventHandler> handler = _handlerMap[@event.GetType()];
            return handler();
        }
    }

    public interface IDomainEventBundle
    {
        void Bundle(IDomainEventRegister register);
    }
}
