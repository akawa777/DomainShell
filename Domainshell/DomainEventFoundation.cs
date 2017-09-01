using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Domainshell
{
    public static class DomainEventPublisher
    {
        private static Action<IDomainEventAuthor> _publish = x => {};
        public static void Publish(IDomainEventAuthor domainEventAuthor)
        {
            _publish(domainEventAuthor);
        }
    }

    public static class DomainEventFoundation
    {
        private static Func<IDomainEventScope> _inTranEventScope;
        private static Func<IDomainEventScope> _outerTranEventScope;
        private static Dictionary<int, List<IDomainEvent>> _outerTranEventMap = new Dictionary<int, List<IDomainEvent>>();
        private static Dictionary<int, List<IDomainEvent>> _exceptionEventMap = new Dictionary<int, List<IDomainEvent>>();

        public static void Startup(Func<IDomainEventScope> inTranEventScope, Func<IDomainEventScope> outerTranEventScope)
        {
            _inTranEventScope = inTranEventScope;
            _outerTranEventScope = outerTranEventScope;

            FieldInfo field = typeof(DomainEventPublisher).GetField("_publish",  BindingFlags.Static | BindingFlags.NonPublic);
            Action<IDomainEventAuthor> publish = x => Publish(x);

            field.SetValue(null, publish);
        }

        public static void ExecOutertran()
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            List<IDomainEvent> cacheEvents;
            if (_outerTranEventMap.TryGetValue(id, out cacheEvents))
            {
                _outerTranEventMap.Remove(id);
                Handle(_outerTranEventScope, cacheEvents.Where(x => !(x as IDomainOuterTranEvent).Async).ToArray());

                if (_exceptionEventMap.ContainsKey(id)) _exceptionEventMap.Remove(id);

                Task.Run(() => Handle(_outerTranEventScope, cacheEvents.Where(x => (x as IDomainOuterTranEvent).Async).ToArray()));
            }
        }

        public static void DealException(Exception exception)
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            if (_outerTranEventMap.ContainsKey(id)) _outerTranEventMap.Remove(id);

            List<IDomainEvent> cacheEvents;
            if (_exceptionEventMap.TryGetValue(id, out cacheEvents))
            {
                _exceptionEventMap.Remove(id);
                Handle(_outerTranEventScope, cacheEvents.ToArray(), exception);
            }
        }

        private static void Publish(IDomainEventAuthor domainEventAuthor)
        {
            var eventSet = GetEvents(domainEventAuthor);

            HandleExceptionEvents(eventSet.exceptionEvents);                
            HandleInTranEvents(eventSet.inTranEvents);
            HandleOuterTranEvents(eventSet.outertranEvents);           
        }          

        private static (IDomainEvent[] inTranEvents, IDomainEvent[] outertranEvents, IDomainEvent[] exceptionEvents) GetEvents(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEvent[] events = domainEventAuthor.GetEvents().ToArray();            
            domainEventAuthor.ClearEvents();

            IDomainEvent[] inTranEvents = events.Where(x => !(x is IDomainOuterTranEvent) && !(x is IDomainExceptionEvent)).ToArray();
            IDomainEvent[] outertranEvents = events.Where(x => (x is IDomainOuterTranEvent) && !(x is IDomainExceptionEvent)).ToArray();            
            IDomainEvent[] exceptionEvents = events.Where(x => x is IDomainExceptionEvent).ToArray();            

            return (inTranEvents, outertranEvents, exceptionEvents);
        }        

        private static void HandleInTranEvents(IDomainEvent[] domainEvents)
        {
            Handle(_inTranEventScope, domainEvents);
        }

        private static void HandleOuterTranEvents(IDomainEvent[] domainEvents)
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            List<IDomainEvent> cacheEvnets;
            if (!_outerTranEventMap.TryGetValue(id, out cacheEvnets))
            {
                cacheEvnets = new List<IDomainEvent>();
                _outerTranEventMap[id] = cacheEvnets;
            }

            cacheEvnets.AddRange(domainEvents);
        }

        private static void HandleExceptionEvents(IDomainEvent[] domainEvents)
        {
            int id = Thread.CurrentThread.ManagedThreadId;

            List<IDomainEvent> cacheEvnets;
            if (!_exceptionEventMap.TryGetValue(id, out cacheEvnets))
            {
                cacheEvnets = new List<IDomainEvent>();
                _exceptionEventMap[id] = cacheEvnets;
            }

            cacheEvnets.AddRange(domainEvents);
        }

        private static void Handle(Func<IDomainEventScope> getScope, IDomainEvent[] domainEvents, Exception exception = null)  
        {
            using (var scope = getScope())
            {
                foreach (IDomainEvent domainEvent in domainEvents)
                {
                    if (domainEvent is IDomainExceptionEvent exceptionEvent) exceptionEvent.Exception = exception;                    
                    Handle(scope, domainEvent);
                }
            }
        }   

        private static void Handle(IDomainEventScope scope, IDomainEvent domainEvent)      
        {            
            object handler = scope.GetType().GetMethod("GetHandler").MakeGenericMethod(domainEvent.GetType()).Invoke(scope, new object[] { domainEvent });

            MethodInfo method = handler.GetType().GetMethod("Handle", new Type[] { domainEvent.GetType() });
            method.Invoke(handler, new object[] { domainEvent });            
        }
    }    
}
