using System;
using System.Linq;
using System.Collections.Generic;
using Domainshell;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Domainshell.Test
{
    public class InTranDomainEventScope : IDomainEventScope
    {
        public InTranDomainEventScope(Container container)
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

    public class OuterTranDomainEventScope : IDomainEventScope
    {
        public OuterTranDomainEventScope(Container container)
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

    public class Session : SessionBase
    {
        public Session(IMemoryConnection connection)
        {
            _connection = connection;
        }

        private IMemoryConnection _connection;

        protected override OpenScopeBase OpenScope()
        {
            return new OpenScope(_connection as MemoryConnection);
        }

        protected override TranScopeBase TranScope()
        {
            return new TranScope(_connection as MemoryConnection);
        }

        protected override void HandleException(Exception exception)
        {
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
            _connection.Open();
            _connection.BeginTran();
        }

        protected override void Commit()
        {            
            _connection.Commit();
            _connection.Close();
        }

        protected override void Rollback()
        {
            _connection.Rollback();
            _connection.Close();
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