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
        public TranScope(Action complete, Action<bool> dispose)
        {                
            _complete = complete;
            _dispose = dispose;
        }

        private Action _complete = () => {};
        private Action<bool> _dispose = x => {};
        private bool _completed = false;

        public void Complete()
        {                   
            _complete();
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

    public abstract class SessionFoundationBase : ISession, IDisposable
    {
        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;
        private object _lockOpen = new object();
        private object _lockTran = new object();

        public virtual IOpenScope Open()
        {
            lock (_lockOpen)
            {
                if (_openScope == null)
                {
                    DomainModelTracker.Revoke();
                    DomainEventPublisher.Revoke();

                    BeginOpen();

                    _openScope = new OpenScope(() =>
                    {
                        lock (_lockOpen)
                        {
                            try
                            {
                                Close();
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

                    _tranScope = new TranScope(() =>
                    {
                        lock (_lockTran)
                        {
                            BeginCommit();
                            PublishDomainEventInTran();
                            Commit();
                            PublishDomainEventOutTran();
                        }
                    },
                    x =>
                    {
                        lock (_lockTran)
                        {
                            try
                            {
                                DisposeTran(x);
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

                return new TranScope(() =>
                {
                    BeginCommit();
                    PublishDomainEventInTran();
                },
                x => { });
            }
        }

        public virtual void Dispose()
        {
            DisposeOpen();
        }

        public virtual void OnException(Exception exception)
        {   
            PublishDomainEventOnException(exception);
        }

        public abstract void BeginOpen();        
        public abstract void BeginTran();
        public abstract void Commit();
        public abstract void Rollback();
        public abstract void DisposeTran(bool completed);
        public abstract void Close();
        public abstract void DisposeOpen();

        protected virtual void BeginCommit()
        {

        }

        protected virtual void PublishDomainEventInTran()
        {
            DomainEventPublisher.PublishInTran();
        }

        protected virtual void PublishDomainEventOutTran()
        {
            DomainEventPublisher.PublishOutTran();
        }

        protected virtual void PublishDomainEventOnException(Exception exception)
        {
            DomainEventPublisher.PublishOnException(exception);
        }
    }   
}