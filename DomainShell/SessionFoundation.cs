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

        public static IOpenScope Open()
        {
            ISession session = _getSession();
            return session.Open();
        }

        public static ITranScope Tran()
        {
            ISession session = _getSession();
            return session.Tran();
        }

        public static void OnException(Exception exception)
        {
            ISession session = _getSession();
            session.OnException(exception);
        }
    }

    public abstract class SessionFoundationBase : ISession
    {
        private class OpenScope : IOpenScope
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

        private class TranScope : ITranScope
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

        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;
        private object _lockOpen = new object();
        private object _lockTran = new object();

        public IOpenScope Open()
        {
            lock (_lockOpen)
            {
                if (_openScope == null)
                {
                    DomainModelTracker.Revoke();
                    DomainEventPublisher.Revoke();

                    IConnection connection = GetConnection();
                    connection.Open();

                    _openScope = new OpenScope(() =>
                    {
                        try
                        {
                            connection.Dispose();
                        }
                        finally
                        {
                            _openScope = null;
                        }
                    });

                    return _openScope;
                }

                return new OpenScope(() => { });
            }
        }

        public ITranScope Tran()
        {
            lock (_lockTran)
            {
                IOpenScope openScope = Open();

                IConnection connection = GetConnection();                

                if (_tranScope == null)
                {
                    connection.BeginTran();

                    _tranScope = new TranScope(() =>
                    {
                        connection.BeginCommit();
                        DomainEventPublisher.PublishInTran();
                        connection.Commit();                        
                        DomainEventPublisher.PublishOutTran();

                    },
                    x =>
                    {
                        try
                        {
                            connection.DisposeTran(x);
                        }
                        finally
                        {
                            _tranScope = null;
                            openScope.Dispose();
                        }
                    });

                    return _tranScope;
                }

                return new TranScope(() =>
                {
                    connection.BeginCommit();
                    DomainEventPublisher.PublishInTran();  
                },
                x => { });
            }
        }

        public void OnException(Exception exception)
        {
            DomainEventPublisher.PublishByException(exception);
        }

        protected abstract IConnection GetConnection();
    }   
}