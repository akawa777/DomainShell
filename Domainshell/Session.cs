using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Domainshell
{
    public interface ISession
    {
        IOpenScope Open();
        ITranScope Tran();
        void OnException(Exception exception);
    }

    public interface IOpenScope : IDisposable
    {
        
    }

    public interface ITranScope : IDisposable
    {
        void Complete();
    }

    public abstract class SessionBase : ISession
    {
        private bool _opned = false;
        private bool _traned = false;

        public IOpenScope Open()
        {
            OpenScopeBase openScope = OpenScope();
            openScope.Init(_opned, () => _opned = false);

            _opned = true;

            return openScope;
        }

        public ITranScope Tran()
        {
            TranScopeBase tranScope = TranScope();
            tranScope.Init(_traned, () => { _opned = false; _traned = false; });

            _opned = true;
            _traned = true;

            return tranScope;
        }

        public void OnException(Exception exception)
        {
            try
            {
                DomainEventFoundation.DealException(exception);
            }
            finally
            {
                HandleException(exception);
            }
        }

        protected abstract OpenScopeBase OpenScope();
        protected abstract TranScopeBase TranScope();
        protected abstract void HandleException(Exception exception);
    }

    public abstract class OpenScopeBase : IOpenScope
    {
        private bool _opned = false;
        private Action _dispose = () => {};

        public void Init(bool opend, Action dispose)
        {
            _opned = opend;

            if (!_opned)
            {
                _dispose = dispose;
                Open();                
            } 
        }

        protected abstract void Open();

        protected abstract void Close();

        public void Dispose()
        {
            if (!_opned) 
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
    }

    public abstract class TranScopeBase : ITranScope
    {
        private bool _traned;
        private Action _dispose = () => {};

        public void Init(bool traned, Action dispose)
        {
            _traned = traned;

            if (!_traned)
            {
                 _dispose = dispose;
                BeginTran();  
            }
        }

        protected abstract void BeginTran();

        protected abstract void Commit();

        protected abstract void Rollback();

        public void Complete()
        {
            if (!_traned)
            {
                Commit();
                DomainEventFoundation.ExecOutertran();
            }
        }

        public void Dispose()
        {
            if (!_traned) 
            {
                try
                {
                    Rollback();
                }
                finally
                {                    
                    _dispose();
                }
            }
        }
    }
}