using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Domain;

namespace DomainShell.Infrastructure
{   
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private Dictionary<Type, Action<IDomainEvent>> _handlerMap = new Dictionary<Type, Action<IDomainEvent>>();

        public void Register<TDomainEvent>(IDomianEventHandler<TDomainEvent> handler) where TDomainEvent : class, IDomainEvent
        {
            _handlerMap[typeof(TDomainEvent)] = domainEvent => handler.Handle(domainEvent as TDomainEvent);
        }

        public void Dispatch(IDomainEvent domainEvent)
        {   
            _handlerMap[domainEvent.GetType()](domainEvent);
        }
    }
}
