using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;

namespace DomainShell.Test.Domains
{
    public class DomainModelProxyFactoryFoundation : DomainModelProxyFactoryFoundationBase
    {
        public DomainModelProxyFactoryFoundation(Container container)
        {
            _container = container;
        }

        private Container _container;

        protected override T CreateProxy<T>()
        {
            return _container.GetInstance<T>();
        }
    }

    public class DomainModelTrackerFoundation : DomainModelTrackerFoundationBase
    {
        protected override object CreateTag<T>(T domainModel)
        {
            return null;
        }
    }

    public class DomainEventFoundation : DomainEventFoundationBase
    {
        public DomainEventFoundation(Container container)
        {
            _container = container;
        }

        private Container _container;

        protected override IDomainEventScope InTranEventScope()
        {
            return new DomainEventScope(_container);
        }

        protected override IDomainEventScope OutTranEventScope()
        {
            return new DomainEventScope(_container, isOutTran: true);
        }
    }

    public class DomainEventScope : IDomainEventScope
    {
        public DomainEventScope(Container container, bool isOutTran = false)
        {
            _container = container;
            if (isOutTran) _scope = ThreadScopedLifestyle.BeginScope(_container);
        }

        private Container _container;
        private Scope _scope;

        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
        }

        public void Dispose()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
            
        }
    }

    public class Connection : IConnection
    {
        public Connection(IDbConnection connection)
        {
            _connection = connection;
        }

        private IDbConnection _connection;

        public IDbCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }
    }

    public class SessionFoundation : SessionFoundationBase
    {
        public SessionFoundation(IDbConnection connection)
        {
            _connection = connection;
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        protected override void BeginOpen()
        {
            _connection.Open();
        }
        protected override void BeginTran()
        {
            _transaction = _connection.BeginTransaction();
        }

        protected override void Save()
        {

        }

        protected override void EndTran(bool completed)
        {
            if (completed)
            {
                _transaction.Commit();                
            }
            else
            {
                _transaction.Rollback();
            }
        }
        protected override void EndOpen()
        {
            _connection.Close();
        }

        public override void Dispose()
        {
            if (_transaction != null) _transaction.Dispose();            
            _connection.Dispose();
        }
    }
}