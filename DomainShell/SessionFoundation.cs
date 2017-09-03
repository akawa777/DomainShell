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
                if (_tranScope != null) _tranScope.Complete();
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

        public IOpenScope Open()
        {
            if (_openScope == null)
            {
                IOpenScope openScope = OpenScopeBase();

                if (openScope is OpenScopeBase scope)
                {
                    _openScope = new OpenScope(scope, () => _openScope = null);
                }
                else
                {
                    _openScope = openScope;
                }
                
                return _openScope;
            }

            return new OpenScope();
        }

        public ITranScope Tran()
        {
            IOpenScope openScope = Open();

            if (_tranScope == null)
            {
                ITranScope tranScope = TranScopeBase();

                if (tranScope is TranScopeBase scope)
                {
                    _tranScope = new TranScope(scope, () =>
                    {
                        _tranScope = null;
                        if (openScope is OpenScopeBase) openScope.Dispose();
                    });
                }
                else
                {
                    _tranScope = tranScope;
                }

                return _tranScope;
            }

            return new TranScope();
        }

        public void OnException(Exception exception)
        {
            DomainEventExceptionPublisher.Publish(exception);
        }

        protected abstract OpenScopeBase OpenScopeBase();
        protected abstract TranScopeBase TranScopeBase();
    }   

    public abstract class OpenScopeBase : IOpenScope
    {
        public abstract void Open();

        protected abstract void Close();

        public abstract void Dispose();
    }

    public abstract class TranScopeBase : ITranScope
    {
        private bool _completed = false;        

        public abstract void BeginTran();

        protected abstract void Commit();

        protected abstract void Dispose(bool completed);

        public void Complete()
        {            
            Commit();
            _completed = true;
        }

        public void Dispose()
        {
            Dispose(_completed);
            _completed = false;
        }
    }
}