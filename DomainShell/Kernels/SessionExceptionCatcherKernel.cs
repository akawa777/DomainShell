using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Kernels
{
    public interface ISessionExceptionCatcherKernel
    {
        void Catch(Exception exception);
    }

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
                HandleDomainEventsOnException(exception, _domainEventCache.ToArray());
                OnException(exception);
            }
            catch(Exception e)
            {
                var aggregateException = new AggregateException(e, exception);
                throw aggregateException;
            }
            finally
            {
                _domainEventCache.Clear();
            }
        }

        protected abstract void OnException(Exception exception);

        protected abstract void HandleDomainEventsOnException(Exception exception, TDomainEvent[] domainEvents);
    }
}
