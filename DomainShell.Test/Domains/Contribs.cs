using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using MediatR;
using System.Threading.Tasks;

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

    public class SessionFoundation : SessionFoundationBase<IDomainEvent>, IConnection
    {
        public SessionFoundation(Container container, IDbConnection connection, IMediator mediator)
        {
            _container = container;
            _connection = connection;
            _mediator = mediator;
        }

        private Container _container;
        private IMediator _mediator;
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

        protected override void OnException(Exception exception, IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent.Mode.Format == DomainEventFormat.AtException)
                {
                    var task = _mediator.Publish(domainEvent);
                }
            }
        }

        protected override void PublishDomainEventInTran(IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent.Mode.Format == DomainEventFormat.InTran)
                {
                    var task = _mediator.Publish(domainEvent);
                }
            }
        }

        protected override void PublishDomainEventOutTran(IDomainEvent[] domainEvents)
        {
            Task.Run(() => 
            { 
                using (var scope = ThreadScopedLifestyle.BeginScope(_container))
                {
                     foreach (var domainEvent in domainEvents)
                     {
                         if (domainEvent.Mode.Format == DomainEventFormat.OutTran)
                         {
                            _mediator.Publish(domainEvent);
                        }
                     }
                }
            });
        }

        public void Dispose()
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
    
    public struct DomainEventMode
    {
        private DomainEventMode(DomainEventFormat format)
        {
            _format = format;
            _exception = null;
        }

        private DomainEventFormat _format;
        public DomainEventFormat Format
        {
            get { return _format; }
            set { _format = value;  }
        }

        private Exception _exception;
        public Exception GetException()
        {
            return _exception;
        }

        public static DomainEventMode InTran()
        {
            return new DomainEventMode(DomainEventFormat.InTran);
        }

        public static DomainEventMode OutTran()
        {
            return new DomainEventMode(DomainEventFormat.OutTran);
        }

        public static DomainEventMode AtException()
        {
            return new DomainEventMode(DomainEventFormat.AtException);
        }
    }

    public enum DomainEventFormat
    {
        InTran,
        OutTran,
        AtException         
    }

    public interface IDomainEvent : INotification
    {
        DomainEventMode Mode { get; }
    }    

    public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        
    }

    public interface IDomainEventAuthor : IDomainEventAuthor<IDomainEvent>
    {

    }
}