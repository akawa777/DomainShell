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
            public OpenScope()
            {
                
            }

            public OpenScope(OpenScopeBase openScope, Action dispose)
            {
                _openScope = openScope;
                _dispose = dispose;

                _openScope.Open();
            }

            private OpenScopeBase _openScope = null;
            private Action _dispose = () => {};

            public void Dispose()
            {
                try
                {
                    if(_openScope != null) _openScope.Dispose();                    
                }
                finally
                {
                    _dispose();
                }
            }
        }

        private class TranScope : ITranScope
        {
            public TranScope()
            {
                
            }

            public TranScope(TranScopeBase openScope, Action dispose)
            {
                _tranScope = openScope;
                _dispose = dispose;

                _tranScope.BeginTran();
            }

            private TranScopeBase _tranScope = null;
            private Action _dispose = () => {};

            public void Complete()
            {
                if (_tranScope != null)
                {
                    _tranScope.Complete();
                }
            }

            public void Dispose()
            {
                try
                {
                    if(_tranScope != null) _tranScope.Dispose();                    
                }
                finally
                {
                    _dispose();
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

                    OpenScopeBase openScope = OpenScopeBase();

                    _openScope = new OpenScope(openScope, () =>
                    {
                        _openScope = null;
                    });

                    return _openScope;
                }

                return new OpenScope();
            }
        }

        public ITranScope Tran()
        {
            lock (_lockTran)
            {
                IOpenScope openScope = Open();

                if (_tranScope == null)
                {
                    TranScopeBase tranScope = TranScopeBase();

                    _tranScope = new TranScope(tranScope, () =>
                    {
                        _tranScope = null;
                        openScope.Dispose();
                    });

                    return _tranScope;
                }

                return InTranScopeBase();
            }
        }

        public void OnException(Exception exception)
        {
            DomainEventPublisher.PublishByException(exception);
        }

        protected abstract OpenScope OpenScopeBase();
        protected abstract TranScopeBase TranScopeBase();
        protected abstract InTranScopeBase InTranScopeBase();
    }   

    public abstract class OpenScopeBase : IOpenScope
    {
        public abstract void Open();

        protected abstract void Close();

        public void Dispose()
        {
            Close();
        }
    }

    public abstract class InTranScopeBase : ITranScope
    {
        private bool _completed = false;
        protected abstract void BeginCommit();
        protected abstract void Dispose(bool completed);

        public void Complete()
        {
            BeginCommit();
            DomainEventPublisher.PublishInTran();
            _completed = true;
        }

        public void Dispose()
        {
            try
            {
                Dispose(_completed);
            }
            finally
            {
                _completed = false;
            }
        }
    }

    public abstract class TranScopeBase : ITranScope
    {
        private bool _completed = false;        

        public abstract void BeginTran();

        protected abstract void BeginCommit();

        protected abstract void Commit();

        protected abstract void Dispose(bool completed);

        public void Complete()
        {   
            BeginCommit();
            DomainEventPublisher.PublishInTran();                     
            Commit();
            _completed = true;
            DomainEventPublisher.PublishOutTran();            
        }

        public void Dispose()
        {
            try
            {
                Dispose(_completed);
            }
            finally
            {
                _completed = false;
            }
        }
    }
}