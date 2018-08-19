﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Kernels
{
    public interface IDomainEventPublisherKernel
    {
        void Publish(object aggregateRoot);
        Type AggregateRootDefineType { get; }
        ICollection<object> DomainEventCache { get; }
    }    

    public abstract class DomainEventPublisherKernelBase<TAggregateRoot, TDomainEvent> : IDomainEventPublisherKernel
    {   
        public Type AggregateRootDefineType => typeof(TAggregateRoot);

        public ICollection<object> DomainEventCache { get; } = new List<object>();

        public void Publish(TAggregateRoot aggregateRoot)
        {
            try
            {
                var domainEvents = GetDomainEvents(aggregateRoot);                

                foreach (var domainEvent in domainEvents)
                {
                    DomainEventCache.Add(domainEvent);
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

        void IDomainEventPublisherKernel.Publish(object aggregateRoot)
        {
            Publish((TAggregateRoot)aggregateRoot);
        }
    }
}
