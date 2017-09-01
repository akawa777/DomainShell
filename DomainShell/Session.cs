using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IOpenScope : IDisposable
    {
        
    }

    public interface ITranScope : IDisposable
    {
        void Complete();
    }

    public static class SessionFoundation
    {
        public static void Startup(Func<OpenScopeBase> openScope, Func<TranScopeBase> tranScope)        
        {
            FieldInfo field = typeof(Session).GetField("_openScope",  BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, openScope);

            field = typeof(Session).GetField("_tranScope",  BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, tranScope);
        }
    }

    public static class Session
    {
        private static Func<OpenScopeBase> _openScope = () => null;
        private static Func<TranScopeBase> _tranScope = () => null;
        private static Dictionary<int, IOpenScope> _openMap = new Dictionary<int, IOpenScope>();
        private static Dictionary<int, ITranScope> _tranMap = new Dictionary<int, ITranScope>();

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

        public static IOpenScope Open()
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            if (_openMap.ContainsKey(id))
            {
                return new OpenScope();
            }

            OpenScopeBase scope = _openScope();
            scope.Init(() => _openMap.Remove(id));
            _openMap[id] = scope;

            return scope;
        }

        public static ITranScope Tran()
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            if (_tranMap.ContainsKey(id))
            {
                return new TranScope();
            }

            TranScopeBase scope = _tranScope();
            scope.Init(() => _tranMap.Remove(id));
            _tranMap[id] = scope;

            return scope;
        }

        public static void OnException(Exception exception)
        {
            DomainEventFoundation.DealException(exception);
        }
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

        public void Complete()
        {
            Commit();
            DomainEventFoundation.ExecOutertran();
        }

        public void Dispose()
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