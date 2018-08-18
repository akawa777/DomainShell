using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public class DomainEventCache : DomainEventCacheKernelBase<IDomainEvent>
    {

    }

    public class DomainEventPublisherKernel : DomainEventPublisherKernelBase<IAggregateRoot, IDomainEvent>
    {
        public DomainEventPublisherKernel(Container container, IDomainEventCacheKernel<IDomainEvent> domainEventCache) : base(domainEventCache)
        {
            _container = container;
        }

        private readonly Container _container;        

        protected override IDomainEvent[] GetDomainEvents(IAggregateRoot aggregateRoot)
        {
            return aggregateRoot.GetDomainEvents();
        }

        protected override void HandleDomainEvents(IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

                if (_container.GetRegistration(handlerType) != null)
                {
                    var handler = _container.GetInstance(handlerType) as dynamic;
                    handler.Handle(domainEvent as dynamic);
                }
            }
        }

        protected override void HandleDomainEventsAsync(IDomainEvent[] domainEvents)
        {
            using (ThreadScopedLifestyle.BeginScope(Bootstrap.Container))
            {
                foreach (var domainEvent in domainEvents)
                {
                    var handlerType = typeof(IDomainEventAsyncHandler<>).MakeGenericType(domainEvent.GetType());

                    if (_container.GetRegistration(handlerType) != null)
                    {
                        var handler = _container.GetInstance(handlerType) as dynamic;
                        handler.Handle(domainEvent as dynamic);
                    }
                }
            }
        }

        protected override void ClearDomainEvents(IAggregateRoot domainEventAuthor)
        {
            domainEventAuthor.ClearDomainEvents();
        }
    }
}