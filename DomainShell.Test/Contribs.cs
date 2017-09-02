using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace DomainShell.Test
{
    public class DomainEventFoundation : DomainEventFoundationBase
    {
        public DomainEventFoundation(Container container)
        {
            _container = container;
        }

        private Container _container;

        protected override IDomainEventScope AsyncEventScope()
        {
            return new AsyncEventScope(_container);
        }

        protected override IDomainEventScope ExceptionEventScope()
        {
            return new ExceptionEventScope(_container);
        }

        protected override IDomainEventScope SyncEventScope()
        {
            return new SyncEventScope(_container);
        }
    }

    public class SyncEventScope : IDomainEventScope
    {
        public SyncEventScope(Container container)
        {
            _container = container;
        }

        private Container _container;

        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
        }

        public void Dispose()
        {

        }
    }

    public class AsyncEventScope : IDomainEventScope
    {
        public AsyncEventScope(Container container)
        {
            _container = container;
            _scope = ThreadScopedLifestyle.BeginScope(_container);
        }

        private Container _container;
        private Scope _scope; 

        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }

    public class ExceptionEventScope : AsyncEventScope
    {
        public ExceptionEventScope(Container container) : base(container)
        {
            
        }
    }

    public class MemorySession : SessionBase
    {
        public MemorySession(MemoryConnection connection)
        {
            _connection = connection;
        }

        private MemoryConnection _connection;

        protected override OpenScopeBase OpenScopeBase()
        {
            return new OpenScope(_connection);
        }

        protected override TranScopeBase TranScopeBase()
        {
            return new TranScope(_connection);
        }
    }

    public class OpenScope : OpenScopeBase
    {
        public OpenScope(MemoryConnection connection)
        {
            _connection = connection;
        }

        private MemoryConnection _connection;

        protected override void Open()
        {
            _connection.Open();
        }

        protected override void Close()
        {
            _connection.Close();
        }
    }

    public class TranScope : TranScopeBase
    {
        public TranScope(MemoryConnection connection)
        {
            _connection = connection;
        }

        private MemoryConnection _connection;
        protected override void BeginTran()
        {            
            _connection.BeginTran();
        }

        protected override void Commit()
        {            
            _connection.Commit();
        }

        protected override void Rollback()
        {
            _connection.Rollback();
        }

        protected override void EndTran()
        {

        }
    }

    public interface IMemoryConnection
    {
        MemoryDatabase Database { get; }
    }

    public class MemoryConnection : IMemoryConnection
    {
        private static MemoryDatabase _memoryDatabase  = new MemoryDatabase();
        private bool _opend = false;
        private bool _traned = false;

        public void Open()
        {
            if (_opend)
            {
                throw new Exception("already open.");
            }

            _opend = true;
        }

        public void Close()
        {
            _opend = false;
        }

        public void BeginTran()
        {
            if (_traned)
            {
                throw new Exception("already tran.");
            }

            _traned = true;
        }

        public void Commit()
        {
            _traned = false;
        }

        public void Rollback()
        {
            if (_traned)
            {
                throw new Exception("not committed.");
            }
        }

        public MemoryDatabase Database 
        { 
            get 
            { 
                if (_opend) return _memoryDatabase; 
                else throw new Exception("connection is close.");
            } 
        }
    }    

    public class MemoryDatabase
    {
        private Dictionary<object, object> _dataMap = new Dictionary<object, object>();

        public IEnumerable<T> Get<T>() where T : class
        {
            return _dataMap.Values.Where(x => x is T).Select(x => (T)x);
        }

        public void Insert<T>(T data) where T : class
        {
            if (_dataMap.ContainsKey(data))
            {
                throw new Exception("data is exists.");
            }

            _dataMap[data] = data;
        }

        public void Update<T>(T data) where T : class
        {
            if (!_dataMap.ContainsKey(data))
            {
                throw new Exception("data not found.");
            }
        }

        public void Delete<T>(T data) where T : class
        {
            if (!_dataMap.ContainsKey(data))
            {
                throw new Exception("data not found.");
            }

            _dataMap.Remove(data);
        }
    }
}