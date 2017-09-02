using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEventPublisher
    {
        void Publish(IDomainEventAuthor domainEventAuthor);
    }

    public interface IDomainEventExceptionPublisher
    {   
        void Publish(Exception exception);
    }

    public static class DomainEventPublisher
    {
        private static Func<IDomainEventPublisher> _getDomainEventPublisher;

        public static void Startup(Func<IDomainEventPublisher> getDomainEventPublisher)
        {
            _getDomainEventPublisher = getDomainEventPublisher;
        }

        public static void Publish(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            domainEventPublisher.Publish(domainEventAuthor);
        }
    }

    public static class DomainEventExceptionPublisher
    {
        private static Func<IDomainEventExceptionPublisher> _getDomainEventExceptionPublisher;

        public static void Startup(Func<IDomainEventExceptionPublisher> getDmainEventExceptionPublisher)
        {
            _getDomainEventExceptionPublisher = getDmainEventExceptionPublisher;
        }

        public static void Publish(Exception exception)
        {
            IDomainEventExceptionPublisher domainEventExceptionPublisher = _getDomainEventExceptionPublisher();
            domainEventExceptionPublisher.Publish(exception);
        }
    }

    public abstract class DomainEventFoundationBase : IDomainEventPublisher, IDomainEventExceptionPublisher
    {
        private List<IDomainExceptionEvent> _domainExceptionEvents = new List<IDomainExceptionEvent>();

        public void Publish(IDomainEventAuthor domainEventAuthor)
        {
            var eventSet = GetEvents(domainEventAuthor);

            Subscribe(eventSet.exceptionEvents);
            Handle(SyncEventScope, eventSet.syncEvents);
            Handle(AsyncEventScope, eventSet.asyncEvents, async: true);            
        }        

        public void Publish(Exception exception)
        {
            IDomainEvent[] events = _domainExceptionEvents.ToArray();
            _domainExceptionEvents.Clear();

            Handle(ExceptionEventScope, events, async: false, exception: exception);
        }

        private static (IDomainEvent[] syncEvents, IDomainEvent[] asyncEvents, IDomainExceptionEvent[] exceptionEvents) GetEvents(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEvent[] events = domainEventAuthor.GetEvents().ToArray();
            domainEventAuthor.ClearEvents();

            Func<IDomainEvent, bool> isSyncEvent = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (e is IDomainAsyncEvent domainEvent && domainEvent.Async) return false;
                else return true;
            };

            Func<IDomainEvent, bool> isAsyncEvent = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (isSyncEvent(e)) return false;
                else return true;
            };

            Func<IDomainEvent, bool> isExceptionEvent = e =>
            {
                return e is IDomainExceptionEvent;
            };

            IDomainEvent[] syncEvents = events.Where(isSyncEvent).ToArray();
            IDomainEvent[] asyncEvents = events.Where(isAsyncEvent).ToArray();
            IDomainExceptionEvent[] exceptionEvents = events.Where(isExceptionEvent).Select(x => x as IDomainExceptionEvent).ToArray();

            return (syncEvents, asyncEvents, exceptionEvents);
        }

        private void Subscribe(IDomainExceptionEvent[] domainExceptionEvents)
        {
            _domainExceptionEvents.AddRange(domainExceptionEvents);
        }

        private void Handle(Func<IDomainEventScope> getScope, IDomainEvent[] domainEvents, bool async = false,  Exception exception = null)
        {
            Action handle = () =>
            {
                using (var scope = getScope())
                {
                    foreach (IDomainEvent domainEvent in domainEvents)
                    {
                        if (domainEvent is IDomainExceptionEvent exceptionEvent) exceptionEvent.Exception = exception;
                        Handle(scope, domainEvent);
                    }
                }
            };

            if (async) Task.Run(handle);
            else handle();
        }

        private void Handle(IDomainEventScope scope, IDomainEvent domainEvent)
        {
            object handler = scope.GetType().GetMethod("GetHandler").MakeGenericMethod(domainEvent.GetType()).Invoke(scope, new object[] { domainEvent });

            MethodInfo method = handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() });
            method.Invoke(handler, new object[] { domainEvent });
        }   

        protected abstract IDomainEventScope SyncEventScope();
        protected abstract IDomainEventScope AsyncEventScope();
        protected abstract IDomainEventScope ExceptionEventScope();
    }
}
