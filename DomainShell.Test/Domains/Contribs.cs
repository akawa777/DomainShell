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

    public class SessionR<TEvent> where TEvent : class
    {
        private ISessionBehavior<TEvent> _behavior;
        private bool _opend;
        private bool _traned;

        private TEvent[] GetEvent()
        {
            return null;
        }

        private void ClearEvents()
        {
            
        }

        public void Open()
        {
            _behavior.Open();
        }

        public void Tran()
        {
            _traned = true;
            _behavior.BeginTran();
        }

        public void Complete()
        {
            _behavior.EndTran(true, GetEvent());
        }

        public void Dispose()
        {
            if (!_traned)
            {
                _behavior.EndTran(false, GetEvent());
            }

            ClearEvents();
            _traned = false;
            _behavior.Close();
        }
    }

    public interface ISessionBehavior<TEvent>  where TEvent : class
    {
        void Open();
        void Close();
        void BeginTran();        
        void EndTran(bool completed, TEvent[] events);
        void Exception(Exception exception, IEvent[] events);
    }    

    public class SessionBehavior : ISessionBehavior<IEvent>
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
       public void EndTran(bool completed, IEvent[] events)
       {
           if (completed)
           {
               foreach (var @event in events.Where(x => x is IEvent || (x is IAsyncEvent asyncEvent && asyncEvent.Sync)))
               {
                   var task = _mediator.Publish(@event);
                   task.Wait();
               }
           }

           _transaction.Commit();
           _transaction.Dispose();
           _transaction = null;

           using (ThreadScopedLifestyle.BeginScope(_container))
           {
                foreach (var @event in events.Where(x => x is IAsyncEvent asyncEvent && !asyncEvent.Sync))
                {
                    _mediator.Publish(@event);
                }
           }
       }

       public IDbCommand CreateCommand()
       {
           IDbCommand command = _connection.CreateCommand();
           command.Transaction = _transaction;

           return command;
       }

       public void Exception(Exception exception, IEvent[] events)
       {
           using (ThreadScopedLifestyle.BeginScope(_container))
           {
                foreach (var @event in events.Where(x => x is IExceptionEvent).Select(x => x as IExceptionEvent))
                {
                    @event.Exception = exception;
                    _mediator.Publish(@event);
                }
           }
       }
    }

    public interface IEventAuthor<TEvent>
    {
       TEvent[] GetEvents();
       void ClearEvents();
    }    

    public class SampleEvent : INotification
    {
        
    }

    public interface IEvent : INotification
    {
        bool Async { get; set; }
    }

    public interface IAsyncEvent : IEvent
    {
       bool Sync { get; set; }
    }

    public interface IExceptionEvent : INotification
    {
       Exception Exception { get; set; }
    }
}