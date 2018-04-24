using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace DomainShell.Test
{
    public class DomainModelFactoryFoundation : DomainModelFactoryFoundationBase
    {
        public DomainModelFactoryFoundation(Container container)
        {
            _container = container;
        }

        private Container _container;

        protected override bool TryCreate<T>(out T model)
        {
            model = default(T);

            var producer = _container.GetRegistration(typeof(T));

            if (producer == null) return false;

            model = _container.GetInstance<T>();

            return true;
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
        public SessionFoundation(Container container, IDbConnection connection)
        {
            _container = container;
            _connection = connection;
        }

        private Container _container;
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

        protected override void PublishDomainEventOnException(Exception exception, IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent.Mode.Format == DomainEventFormat.OnException)
                {
                    var field = domainEvent.Mode.GetType().GetField("_exception", BindingFlags.Instance | BindingFlags.NonPublic);

                    field.SetValue(domainEvent.Mode, exception);

                    var handler = _container.GetInstance(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

                    handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() }).Invoke(handler, new object[] { domainEvent });
                }
            }
        }

        protected override void PublishDomainEventInSession(IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (domainEvent.Mode.Format == DomainEventFormat.InSession)
                {
                    var handler = _container.GetInstance(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

                    handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() }).Invoke(handler, new object[] { domainEvent });
                }
            }
        }

        protected override void PublishDomainEventOutSession(IDomainEvent[] domainEvents)
        {
            Task.Run(() => 
            { 
                using (var scope = ThreadScopedLifestyle.BeginScope(_container))
                {
                     foreach (var domainEvent in domainEvents)
                     {
                         if (domainEvent.Mode.Format == DomainEventFormat.OutSession)
                         {
                            var handler = _container.GetInstance(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

                            handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() }).Invoke(handler, new object[] { domainEvent });
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
            var command = _connection.CreateCommand();

            if (_transaction != null) command.Transaction = _transaction;

            return command;
        }
    }
    
    public class DomainEventMode
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

        public static DomainEventMode InSession()
        {
            return new DomainEventMode(DomainEventFormat.InSession);
        }

        public static DomainEventMode OutSession()
        {
            return new DomainEventMode(DomainEventFormat.OutSession);
        }

        public static DomainEventMode OnException()
        {
            return new DomainEventMode(DomainEventFormat.OnException);
        }
    }

    public enum DomainEventFormat
    {
        InSession,
        OutSession,
        OnException         
    }

    public interface IDomainEvent
    {
        DomainEventMode Mode { get; }
    }    

    public interface IDomainEventHandler<TDomainEvent>
    {
        void Handle(TDomainEvent domainEvent);
    }

    public abstract class AggregateRoot : IAggregateRoot, IDomainEventAuthor<IDomainEvent>
    {
        protected AggregateRoot()
        {
            DomainModelTracker.Mark(this);
        }

        protected List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            return DomainEvents.AsReadOnly();
        }

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }

        public bool Deleted { get; protected set; }

        public ModelState State { get; protected set; }

        public string LastUpdate { get; private set; }
    }

    public abstract class ReadAggregateRoot : IDomainEventAuthor<IDomainEvent>
    {
        protected ReadAggregateRoot()
        {
            DomainModelTracker.Mark(this);
        }

        protected List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public IEnumerable<IDomainEvent> GetDomainEvents()
        {
            return DomainEvents.AsReadOnly();
        }

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }
    }

    public static class Log
    {
        public static List<string> MessageList { get; private set; } = new List<string>();
    }
}