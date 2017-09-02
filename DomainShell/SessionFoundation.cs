using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public abstract class SessionFoundationBase : ISession
    {
        private class OpenScope : IOpenScope
        {
            public void Dispose()
            {
            }
        }

        private class TranScope : ITranScope
        {
            public void Complete()
            {

            }

            public void Dispose()
            {
            }
        }

        private IOpenScope _openScope = null;
        private ITranScope _tranScope = null;

        public IOpenScope Open()
        {
            if (_openScope == null)
            {
                OpenScopeBase openScope = OpenScopeBase();
                _openScope = openScope;

                openScope.Init(() => _openScope = null);

                return openScope;
            }

            return new OpenScope();
        }

        public ITranScope Tran()
        {
            IOpenScope openScope = Open();

            if (_tranScope == null)
            {
                TranScopeBase tranScope = TranScopeBase();
                _tranScope = tranScope;

                tranScope.Init(() =>
                {
                    _tranScope = null;
                    if (openScope is OpenScopeBase) openScope.Dispose();
                });

                return tranScope;
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
        private Action _dispose = () => {};

        public void Init(Action dispose)
        {
            _dispose = dispose;
            Open();                
        }

        protected abstract void Open();

        protected abstract void Close();

        public void Dispose()
        {
            try
            {
                Close();                    
            }
            finally
            {                    
                _dispose();                    
            } 
        }
    }

    public abstract class TranScopeBase : ITranScope
    {
        private Action _dispose = () => {};

        public void Init(Action dispose)
        {
            _dispose = dispose;
            BeginTran();  
        }

        protected abstract void BeginTran();

        protected abstract void Commit();

        protected abstract void Rollback();

        protected abstract void EndTran();

        public void Complete()
        {
            Commit();
        }

        public void Dispose()
        {
            try
            {
                Rollback();
                EndTran();
            }
            finally
            {                    
                _dispose();
            }
        }
    }
}