﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Kernels
{
    public abstract class DomainEventCacheKernelBase<TDomainEvent> : List<TDomainEvent>, IDomainEventCacheKernel<TDomainEvent>
    {

    }    

    public abstract class DomainEventPublisherKernelBase<TAggregateRoot, TDomainEvent> : IDomainEventPublisherKernel<TAggregateRoot>
    {   
        public DomainEventPublisherKernelBase(IDomainEventCacheKernel<TDomainEvent> domainEventCache)
        {
            _domainEventCache = domainEventCache;
        }

        private readonly IDomainEventCacheKernel<TDomainEvent> _domainEventCache;        

        public void Publish(TAggregateRoot aggregateRoot)
        {
            try
            {
                var domainEvents = GetDomainEvents(aggregateRoot);                

                foreach (var domainEvent in domainEvents)
                {
                    _domainEventCache.Add(domainEvent);
                }

                HandleDomainEvents(domainEvents);

                Task.Run(() =>
                {
                    HandleDomainEventsAsync(domainEvents);
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                ClearDomainEvents(aggregateRoot);
            }
        }

        protected abstract TDomainEvent[] GetDomainEvents(TAggregateRoot aggregateRoot);        
        protected abstract void HandleDomainEvents(TDomainEvent[] domainEvents);
        protected abstract void HandleDomainEventsAsync(TDomainEvent[] domainEvents);
        protected abstract void ClearDomainEvents(TAggregateRoot aggregateRoot);
    }
}