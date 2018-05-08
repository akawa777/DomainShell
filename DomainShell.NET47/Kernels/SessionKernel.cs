using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell.Kernels
{
    public interface ISessionKernel
    {
        IOpenScope Open();
        ITranScope Tran();
        void OnException(Exception exception);
    }

    internal class OpenScope : IOpenScope
    {
        public OpenScope(Action dispose)
        {
            _dispose = dispose;
        }

        private Action _dispose = () => { };

        public void Dispose()
        {
            _dispose();
        }
    }

    internal class TranScope : ITranScope
    {
        public TranScope(Action<bool> dispose)
        {
            _dispose = dispose;
        }

        private Action<bool> _dispose = x => { };
        private bool _completed = false;

        public void Complete()
        {
            _completed = true;
        }

        public void Dispose()
        {
            try
            {
                _dispose(_completed);
            }
            finally
            {
                _completed = false;
            }
        }
    }

    public abstract class SessionKernelBase<TDomainEvent> : ISessionKernel where TDomainEvent : class
    {
        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;
        private object _lockOpen = new object();
        private object _lockTran = new object();
        private List<TDomainEvent> _allDomainEvents = new List<TDomainEvent>();

        public virtual IOpenScope Open()
        {
            lock (_lockOpen)
            {
                if (_openScope == null)
                {
                    BeginOpen();

                    _openScope = new OpenScope(() =>
                    {
                        lock (_lockOpen)
                        {
                            try
                            {
                                var domainEvents = GetDomainEvents().ToArray();
                                _allDomainEvents.AddRange(domainEvents);

                                PublishDomainEventInSession(domainEvents);

                                EndOpen();

                                PublishDomainEventOutSession(_allDomainEvents.ToArray());
                            }
                            finally
                            {
                                _openScope = null;
                            }
                        }
                    });

                    return _openScope;
                }

                return new OpenScope(() =>
                {
                    var domainEvents = GetDomainEvents().ToArray();
                    _allDomainEvents.AddRange(domainEvents);

                    PublishDomainEventInSession(domainEvents);
                });
            }
        }

        public virtual ITranScope Tran()
        {
            lock (_lockTran)
            {
                var openScope = Open();

                if (_tranScope == null)
                {
                    BeginTran();

                    _tranScope = new TranScope(
                    completed =>
                    {
                        lock (_lockTran)
                        {
                            try
                            {
                                var domainEvents = GetDomainEvents().ToArray();
                                _allDomainEvents.AddRange(domainEvents);

                                if (completed)
                                {
                                    PublishDomainEventInSession(domainEvents);
                                }

                                EndTran(completed);

                                if (completed)
                                {
                                    PublishDomainEventOutSession(_allDomainEvents.ToArray());
                                }
                            }
                            finally
                            {
                                _tranScope = null;
                                openScope.Dispose();
                            }
                        }
                    });

                    return _tranScope;
                }

                return new TranScope(
                completed =>
                {
                    var domainEvents = GetDomainEvents().ToArray();
                    _allDomainEvents.AddRange(domainEvents);

                    if (completed)
                    {
                        PublishDomainEventInSession(domainEvents);
                    }
                });
            }
        }

        protected virtual IEnumerable<TDomainEvent> GetDomainEvents()
        {
            foreach (var trackPack in DomainModelTracker.GetAll())
            {
                var domainEvents = GetDomainEvents(trackPack.DomainModel);

                if (domainEvents != null)
                {
                    foreach (var domainEvent in domainEvents) yield return domainEvent;
                    ClearDomainEvents(trackPack.DomainModel);
                }
            }
        }

        public virtual void OnException(Exception exception)
        {
            var domainEvents = GetDomainEvents().ToArray();
            _allDomainEvents.AddRange(domainEvents);

            PublishDomainEventOnException(exception, _allDomainEvents.ToArray());
        }

        protected abstract void BeginOpen();
        protected abstract void BeginTran();
        protected abstract void EndTran(bool completed);
        protected abstract void EndOpen();
        protected abstract TDomainEvent[] GetDomainEvents(object model);
        protected abstract void ClearDomainEvents(object model);
        protected abstract void PublishDomainEventOnException(Exception exception, TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventInSession(TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventOutSession(TDomainEvent[] domainEvents);
    }
}