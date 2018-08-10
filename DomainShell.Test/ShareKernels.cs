using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public class DomainModelFactoryKernel : DomainModelFactoryKernelBase
    {
        public DomainModelFactoryKernel(Container container)
        {
            _container = container;
        }

        private Container _container;

        public override bool TryCreate(Type type, out object domainModel)
        {
            domainModel = null;

            var producer = _container.GetRegistration(type);

            if (producer == null) return false;

            domainModel = _container.GetInstance(type);

            return true;
        }
    }

    public class DomainModelTrackerKernel : DomainModelTrackerKernelBase
    {
        protected override object CreateTag(object domainModel)
        {
            return null;
        }
    }

    public class SessionKernel : SessionKernelBase<IDomainEvent>, IConnection
    {
        public SessionKernel(Container container, IDbConnection connection)
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
                if (_container.GetRegistration(typeof(IDomainEventExceptionHandler<>).MakeGenericType(domainEvent.GetType())) == null)
                {
                    continue;
                }

                var handler = _container.GetInstance(typeof(IDomainEventExceptionHandler<>).MakeGenericType(domainEvent.GetType()));

                handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType(), typeof(Exception) }).Invoke(handler, new object[] { domainEvent, exception });
            }
        }

        protected override void PublishDomainEventInSession(IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                if (_container.GetRegistration(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType())) == null)
                {
                    continue;
                }

                var handler = _container.GetInstance(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));                

                handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() }).Invoke(handler, new object[] { domainEvent });
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
                        if (_container.GetRegistration(typeof(IDomainEventAsyncHandler<>).MakeGenericType(domainEvent.GetType())) == null)
                        {
                            continue;
                        }

                        var handler = _container.GetInstance(typeof(IDomainEventAsyncHandler<>).MakeGenericType(domainEvent.GetType()));

                        handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() }).Invoke(handler, new object[] { domainEvent });
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

        protected override IDomainEvent[] GetDomainEvents(object model)
        {
            if (model is IDomainEventAuthor domainEventAuthor)
            {
                return domainEventAuthor.GetDomainEvents();
            }

            return new IDomainEvent[0];
        }

        protected override void ClearDomainEvents(object model)
        {
            if (model is IDomainEventAuthor domainEventAuthor)
            {
                domainEventAuthor.ClearDomainEvents();
            }
        }
    }
    
    //public class DomainEventMode
    //{
    //    private DomainEventMode(DomainEventFormat format)
    //    {
    //        _format = format;
    //        _exception = null;
    //    }

    //    private DomainEventFormat _format;
    //    public DomainEventFormat Format
    //    {
    //        get { return _format; }
    //        set { _format = value;  }
    //    }

    //    private Exception _exception;
    //    public Exception GetException()
    //    {
    //        return _exception;
    //    }

    //    public static DomainEventMode InSession()
    //    {
    //        return new DomainEventMode(DomainEventFormat.InSession);
    //    }

    //    public static DomainEventMode OutSession()
    //    {
    //        return new DomainEventMode(DomainEventFormat.OutSession);
    //    }

    //    public static DomainEventMode OnException()
    //    {
    //        return new DomainEventMode(DomainEventFormat.OnException);
    //    }
    //}

    //public enum DomainEventFormat
    //{
    //    InSession,
    //    OutSession,
    //    OnException         
    //}

    //public interface IDomainEvent
    //{
    //    DomainEventMode Mode { get; }
    //}

    public interface IDomainEvent
    {
        
    }

    public interface IDomainEventHandler<TDomainEvent>
    {
        void Handle(TDomainEvent domainEvent);
    }

    public interface IDomainEventAsyncHandler<TDomainEvent>
    {
        void Handle(TDomainEvent domainEvent);
    }

    public interface IDomainEventExceptionHandler<TDomainEvent>
    {
        void Handle(TDomainEvent domainEvent, Exception exception);
    }

    public interface IDomainEventAuthor
    {
        IDomainEvent[] GetDomainEvents();

        void ClearDomainEvents();
    }

    public abstract class AggregateRoot : IAggregateRoot, IDomainEventAuthor
    {
        protected AggregateRoot()
        {
            DomainModelTracker.Mark(this);
        }

        protected List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public IDomainEvent[] GetDomainEvents()
        {
            return DomainEvents.ToArray();
        }

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }

        public bool Deleted { get; protected set; }

        public ModelState State { get; protected set; }

        public string LastUpdate { get; private set; }
    }

    public abstract class ReadAggregateRoot : IAggregateRootRead, IDomainEventAuthor
    {
        protected ReadAggregateRoot()
        {
            DomainModelTracker.Mark(this);
        }

        protected List<IDomainEvent> DomainEvents { get; } = new List<IDomainEvent>();

        public IDomainEvent[] GetDomainEvents()
        {
            return DomainEvents.ToArray();
        }

        public void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }
    }

    public static class Log
    {
        private static Action<string> _handle = x => { };

        public static void SetMessage(string message)
        {
            _messageList.Add(message);
            _handle(message);
        }

        public static void HandleMessage(Action<string> handle)
        {
            _handle = handle;
        }

        private static List<string> _messageList { get; } = new List<string>();
        public static string[] MessageList => _messageList.ToArray();
    }
}