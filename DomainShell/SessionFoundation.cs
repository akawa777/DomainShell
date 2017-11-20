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

        public static void Startup(Func<ISession> getSessionPerThread)
        {
            _getSession = getSessionPerThread;            
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

            ISession session = _getSession();            
            return session.Open();
        }

        public static ITranScope Tran()
        {
            Validate();

            ISession session = _getSession();
            return session.Tran();
        }

        public static void OnException(Exception exception)
        {
            Validate();

            ISession session = _getSession();
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
                                EndOpen();
                            }
                            finally
                            {
                                _openScope = null;
                            }
                        }
                    });

                    return _openScope;
                }

                return new OpenScope(() => { });
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
                                TDomainEvent[] domainEvents = GetDomainEvents().ToArray();
                                _allDomainEvents.AddRange(domainEvents);

                                if (completed)
                                {                                    
                                    PublishDomainEventInTran(domainEvents);                               
                                }
                                
                                EndTran(completed);

                                if (completed)
                                {
                                    PublishDomainEventOutTran(_allDomainEvents.ToArray());
                                }
                            }
                            finally
                            {
                                _tranScope = null;
                                _allDomainEvents.Clear();
                                openScope.Dispose();
                            }
                        }
                    });

                    return _tranScope;
                }

                return new TranScope(
                completed => 
                {
                    TDomainEvent[] domainEvents = GetDomainEvents().ToArray();
                    _allDomainEvents.AddRange(domainEvents);

                    if (completed)
                    { 
                        PublishDomainEventInTran(domainEvents);
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
                    foreach (var domainEvent in domainEvenAuthor.GetEvents()) yield return domainEvent;
                    domainEvenAuthor.ClearEvents();
                }
            }
        }

        public virtual void OnException(Exception exception)
        {
            TDomainEvent[] domainEvents = GetDomainEvents().ToArray();
            _allDomainEvents.AddRange(domainEvents);

            OnException(exception, _allDomainEvents.ToArray());
        }

        protected abstract void BeginOpen();
        protected abstract void BeginTran();
        protected abstract void EndTran(bool completed);
        protected abstract void EndOpen();
        protected abstract void OnException(Exception exception, TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventInTran(TDomainEvent[] domainEvents);
        protected abstract void PublishDomainEventOutTran(TDomainEvent[] domainEvents);        
    }   
}