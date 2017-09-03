using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
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

    public static class DomainAsyncEventPublisher
    {
        private static Func<IDomainAsyncEventPublisher> _getDomainAsyncEventPublisher;

        public static void Startup(Func<IDomainAsyncEventPublisher> getDomainAsyncEventPublisher)
        {
            _getDomainAsyncEventPublisher = getDomainAsyncEventPublisher;
        }

        public static void Publish()
        {
            IDomainAsyncEventPublisher domainAsyncEventPublisher = _getDomainAsyncEventPublisher();
            domainAsyncEventPublisher.Publish();
        }
    }

    public static class DomainExceptionEventPublisher
    {
        private static Func<IDomainExceptionEventPublisher> _getDomainExceptionEventPublisher;

        public static void Startup(Func<IDomainExceptionEventPublisher> getDomainExceptionEventPublisher)
        {
            _getDomainExceptionEventPublisher = getDomainExceptionEventPublisher;
        }

        public static void Publish(Exception exception)
        {
            IDomainExceptionEventPublisher domainExceptionEventPublisher = _getDomainExceptionEventPublisher();
            domainExceptionEventPublisher.Publish(exception);
        }
    }

    public abstract class DomainEventFoundationBase : IDomainEventPublisher, IDomainAsyncEventPublisher, IDomainExceptionEventPublisher
    {
        private List<IDomainAsyncEvent> _domainAsyncEvents = new List<IDomainAsyncEvent>();        
        private List<IDomainExceptionEvent> _domainExceptionEvents = new List<IDomainExceptionEvent>();

        public void Publish(IDomainEventAuthor domainEventAuthor)
        {
            var eventSet = GetEvents(domainEventAuthor);

            SubscribeAsyncEvents(eventSet.asyncEvents);
            SubscribeExceptionEvents(eventSet.exceptionEvents);
            HandleEvents(SyncEventScope, eventSet.syncEvents);                       
        }       

        public void Publish()
        {
            IDomainAsyncEvent[] events = _domainAsyncEvents.ToArray();
            _domainAsyncEvents.Clear(); 

            IDomainEvent[] syncEvents = events.Where(x => !x.Async).ToArray();
            IDomainEvent[] asyncEvents = events.Where(x => x.Async).ToArray();

            HandleEvents(SyncEventScope, syncEvents);      
            HandleEvents(SyncEventScope, asyncEvents, async: true);      
        } 

        public void Publish(Exception exception)
        {
            IDomainExceptionEvent[] events = _domainExceptionEvents.ToArray();
            _domainExceptionEvents.Clear();

            HandleEvents(ExceptionEventScope, events, async: false, exception: exception);
        }

        private static (IDomainEvent[] syncEvents, IDomainAsyncEvent[] asyncEvents, IDomainExceptionEvent[] exceptionEvents) GetEvents(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEvent[] events = domainEventAuthor.GetEvents().ToArray();
            domainEventAuthor.ClearEvents();

            Func<IDomainEvent, bool> isSyncEvent = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (e is IDomainAsyncEvent) return false;
                else return true;
            };

            Func<IDomainEvent, bool> isAsyncEvent = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (!(e is IDomainAsyncEvent)) return false;
                else return true;
            };

            Func<IDomainEvent, bool> isExceptionEvent = e =>
            {
                return e is IDomainExceptionEvent;
            };

            IDomainEvent[] syncEvents = events.Where(isSyncEvent).ToArray();
            IDomainAsyncEvent[] asyncEvents = events.Where(isAsyncEvent).ToArray().Select(x => x as IDomainAsyncEvent).ToArray();
            IDomainExceptionEvent[] exceptionEvents = events.Where(isExceptionEvent).Select(x => x as IDomainExceptionEvent).ToArray();

            return (syncEvents, asyncEvents, exceptionEvents);
        }

        private void SubscribeAsyncEvents(IDomainAsyncEvent[] domainAsyncEvents)
        {
            _domainAsyncEvents.AddRange(domainAsyncEvents);
        }

        private void SubscribeExceptionEvents(IDomainExceptionEvent[] domainExceptionEvents)
        {
            _domainExceptionEvents.AddRange(domainExceptionEvents);
        }

        private void HandleEvents(Func<IDomainEventScope> getScope, IDomainEvent[] domainEvents, bool async = false,  Exception exception = null)
        {
            Action handle = () =>
            {
                using (var scope = getScope())
                {
                    foreach (IDomainEvent domainEvent in domainEvents)
                    {
                        if (domainEvent is IDomainExceptionEvent exceptionEvent) exceptionEvent.Exception = exception;
                        HandleEvent(scope, domainEvent);
                    }
                }
            };

            if (async) Task.Run(handle);
            else handle();
        }

        private void HandleEvent(IDomainEventScope scope, IDomainEvent domainEvent)
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
