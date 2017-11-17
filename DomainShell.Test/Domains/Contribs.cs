using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using MediatR;

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

    public class SessionFoundation : SessionFoundationBase, IConnection
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
            if (_transaction != null) _transaction.Dispose();
            _transaction = null;

            _connection.Close();
        }

        public override void Dispose()
        {                      
            _connection.Dispose();
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = _connection.CreateCommand();

            if (_transaction != null) command.Transaction = _transaction;

            return command;
        }
    }

    public class SessionBehavior
    {
        private Container _container;
        private IMediator _mediator;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public void Open()
        {
            _connection.Open();
        }
        public void Close()
        {
            _connection.Close();
        }

        public void BeginTran()
        {
            _transaction = _connection.BeginTransaction();
        }
        public void EndTran(bool completed)
        {
            if (completed)
            {
                foreach (var trakPack in DomainModelTracker.GetAll())
                {
                    if (trakPack.Model is IEventAuthor eventAuthor)
                    {              
                        foreach (var IEvent )          
                        
                        var task = _mediator.Publish(new SampleEvent());
                        task.RunSynchronously();
                        
                    }
                }
            }
            else
            {

            }

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;

            return command;
        }

        public void Exception()
        {

        }
    }

    public interface IEventAuthor
    {
        IEvent[] GetEvents();
        void ClearEvents();
    }    

    public class SampleEvent : INotification
    {
        
    }

    public class IAsyncNotification : INotification
    {
        public bool Sync { get; set; }
    }

    public class IExceptionNotification : INotification
    {
        Exception Exception { get; set; }
    }
}