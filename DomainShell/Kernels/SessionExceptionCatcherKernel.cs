using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Kernels
{
    public abstract class SessionExceptionCatcherKernelBase<TDomainEvent> : ISessionExceptionCatcherKernel
    {
        public SessionExceptionCatcherKernelBase(IDomainEventCacheKernel<TDomainEvent> domainEventCache)
        {
            _domainEventCache = domainEventCache;
        }

        private readonly IDomainEventCacheKernel<TDomainEvent> _domainEventCache;

        public void Catch(Exception exception)
        {
            try
            {
                HandleDomainEventsOnException(_domainEventCache.ToArray());
                OnException(exception);
            }
            catch(Exception e)
            {
                var aggregateException = new AggregateException(exception, e);
                OnException(aggregateException);
            }
            finally
            {
                _domainEventCache.Clear();
            }
        }

        protected abstract void OnException(Exception exception);

        protected abstract void HandleDomainEventsOnException(TDomainEvent[] domainEvents);
    }
}
