using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public static class Session
    {
        private static Func<ISession> _getSession;

        public static void Startup(Func<ISession> getSession)
        {
            _getSession = getSession;            
        }

        private static void Validate()
        {
            if (_getSession == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static IOpenScope Open()
        {
            Validate();

            var session = _getSession();            
            return session.Open();
        }

        public static ITranScope Tran()
        {
            Validate();

            var session = _getSession();
            return session.Tran();
        }

        public static void OnException(Exception exception)
        {
            Validate();

            var session = _getSession();
            session.OnException(exception);
        }
    }

    internal class OpenScope : IOpenScope
    {
        public OpenScope(Action dispose)
        {
            _dispose = dispose;
        }

        private Action _dispose = () => {};

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

        private Action<bool> _dispose = x => {};
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

    public abstract class SessionFoundationBase<TDomainEvent> : ISession where TDomainEvent : class
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
                IOpenScope openScope = Open();

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
                if (trackPack.Model is IDomainEventAuthor<TDomainEvent> domainEvenAuthor)
                {
                    foreach (var domainEvent in domainEvenAuthor.GetDomainEvents()) yield return domainEvent;
                    domainEvenAuthor.ClearDomainEvents();
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
        protected abstract void PublishDomainEventOnException(Exception exception, TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventInSession(TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventOutSession(TDomainEvent[] domainEvents);        
    }   
}