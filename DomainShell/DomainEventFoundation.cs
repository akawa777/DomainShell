using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public static class DomainEventList
    {
        private static Func<IDomainEventList> _getDomainEventList;

        public static void Startup(Func<IDomainEventList> getDomainEventList)
        {
            _getDomainEventList = getDomainEventList;
        }

        public static void Add(IDomainEvent[] domainEvents)
        {
            IDomainEventList domainEventList = _getDomainEventList();
            domainEventList.Add(domainEvents);
        }

        public static void Add(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEventList domainEventList = _getDomainEventList();
            domainEventList.Add(domainEventAuthor);
        }
    }
    
    public static class DomainEventPublisher
    {
        private static Func<IDomainEventPublisher> _getDomainEventPublisher;

        public static void Startup(Func<IDomainEventPublisher> getDomainEventPublisher)
        {            
            _getDomainEventPublisher = getDomainEventPublisher;
        }

        public static void PublishInTran()
        {
            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishInTran();
        }

        public static void PublishOutTran()
        {
            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishOutTran();
        }

        public static void PublishByException(Exception exception)
        {
            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishByException(exception);
        }
    }

    public abstract class DomainEventFoundationBase : IDomainEventList, IDomainEventPublisher
    {
        List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public  void Add(IDomainEvent[] domainEvents)
        {
            _domainEvents.AddRange(domainEvents);
        }

        public  void Add(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEvent[] domainEvents = domainEventAuthor.GetEvents().ToArray();
            domainEventAuthor.ClearEvents();

            Add(domainEvents);
        }

        public IEnumerable<IDomainEvent> GetInTranEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (e is IDomainOutTranEvent) return false;
                else return true;
            };

            return _domainEvents.Where(isSatisfy);
        }

        public IEnumerable<IDomainEvent> GetOutTranEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (e is IDomainOutTranEvent outTranEvent && !outTranEvent.Async)  return true;
                else return false;
            };

            return _domainEvents.Where(isSatisfy);
        }

        public IEnumerable<IDomainEvent> GetOutTranAsyncEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                if (e is IDomainExceptionEvent) return false;
                if (e is IDomainOutTranEvent outTranEvent && outTranEvent.Async)  return true;
                else return false;
            };

            return _domainEvents.Where(isSatisfy);
        }

        public IEnumerable<IDomainEvent> GetExceptionEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                if (e is IDomainExceptionEvent) return true;
                else return false;
            };

            return _domainEvents.Where(isSatisfy);
        }

        public void Remove(params IDomainEvent[] domainEvents)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                _domainEvents.Remove(domainEvent);
            }
        }

        public void PublishInTran()
        {            
            IDomainEvent[] domainEvents = GetInTranEvents().ToArray();
            Remove(domainEvents);
            
            HandleEvents(InTranEventScope, domainEvents);                       
        }       

        public void PublishOutTran()
        {            
            IDomainEvent[] domainEvents = GetOutTranEvents().ToArray();
            Remove(domainEvents);

            IDomainEvent[] domainAsyncEvents = GetOutTranAsyncEvents().ToArray();
            Remove(domainAsyncEvents);            
            
            HandleEvents(OutTranEventScope, domainEvents);      
            HandleEvents(OutTranEventScope, domainAsyncEvents, async: true);      
        } 

        public void PublishByException(Exception exception)
        {            
            IDomainEvent[] domainEvents = GetExceptionEvents().ToArray();
            Remove(domainEvents);
            

            Dictionary<IDomainEvent, IDomainEvent> domainEventMap = new Dictionary<IDomainEvent, IDomainEvent>();

            foreach (IDomainEvent domainEvent in domainEvents)
            {
                domainEventMap[domainEvent] = domainEvent;
            }

            foreach (IDomainEventAuthor auter in DomainModelTracker.Get<IDomainEventAuthor>())
            {
                foreach (IDomainEvent domainEvent in auter.GetEvents().Where(x => x is IDomainExceptionEvent))
                {
                    domainEventMap[domainEvent] = domainEvent;
                }

                auter.ClearEvents();
            }

            HandleEvents(ExceptionEventScope, domainEventMap.Values.ToArray(), async: false, exception: exception);
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

        protected abstract IDomainEventScope InTranEventScope();
        protected abstract IDomainEventScope OutTranEventScope();
        protected abstract IDomainEventScope ExceptionEventScope();
    }
}
