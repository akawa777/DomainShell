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

        public static void Startup(Func<IDomainEventPublisher> getDomainEventPublisherPerThread)
        {            
            _getDomainEventPublisher = getDomainEventPublisherPerThread;
        }

        private static void Validate()
        {
            if (_getDomainEventPublisher == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static void PublishInTran()
        {
            Validate();

            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishInTran();
        }

        public static void PublishOutTran()
        {
            Validate();

            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishOutTran();
        }

        public static void PublishOnException(Exception exception)
        {
            Validate();

            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();
            
            domainEventPublisher.PublishOnException(exception);
        }

        public static void Revoke()
        {
            Validate();

            IDomainEventPublisher domainEventPublisher = _getDomainEventPublisher();

            domainEventPublisher.Revoke();
        }
    }

    public abstract class DomainEventFoundationBase : IDomainEventPublisher
    {
        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        private object _lock = new object();

        public virtual void PublishInTran()
        {            
            IDomainEvent[] domainEvents = GetInTranEvents().ToArray();
            Remove(domainEvents);
            
            HandleEvents(InTranEventScope, domainEvents);                       
        }       

        public virtual void PublishOutTran()
        {            
            IDomainEvent[] domainEvents = GetOutTranEvents().ToArray();
            Remove(domainEvents);

            IDomainEvent[] domainAsyncEvents = GetOutTranAsyncEvents().ToArray();
            Remove(domainAsyncEvents);            
            
            HandleEvents(OutTranEventScope, domainEvents);      
            HandleEvents(OutTranEventScope, domainAsyncEvents, async: true);      
        } 

        public virtual void PublishOnException(Exception exception)
        {            
            IDomainEvent[] domainEvents = GetExceptionEvents().ToArray();
            Remove(domainEvents);

            HandleEvents(ExceptionEventScope, domainEvents, async: false, exception: exception);
        }

        public virtual void Revoke()
        {
            _domainEvents.Clear();
        }

        private void HandleEvents(Func<IDomainEventScope> getScope, IDomainEvent[] domainEvents, bool async = false,  Exception exception = null)
        {
            Action handle = () =>
            {
                using (var scope = getScope())
                {
                    foreach (IDomainEvent domainEvent in domainEvents)
                    {
                        if (domainEvent.Mode.Format == DomainEventFormat.AtException && exception != null)
                        {
                            FieldInfo field = domainEvent.Mode.GetType().GetField("_exception", BindingFlags.Instance | BindingFlags.NonPublic);
                            field.SetValue(domainEvent.Mode, exception);
                        }

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

        private IDomainEvent[] GetDomainEvents()
        {
            lock (_lock)
            {
                foreach (IDomainEventAuthor author in GetTargetDomainEventAuthors())
                {
                    foreach (IDomainEvent domainEvent in author.GetEvents())
                    {
                        _domainEvents.Add(domainEvent);
                    }

                    author.ClearEvents();
                }

                return _domainEvents.ToArray();
            }
        }

        private void Add(IDomainEvent[] domainEvents)
        {
            _domainEvents.AddRange(domainEvents);
        }

        private void Add(IDomainEventAuthor domainEventAuthor)
        {
            IDomainEvent[] domainEvents = domainEventAuthor.GetEvents().ToArray();
            domainEventAuthor.ClearEvents();

            Add(domainEvents);
        }

        private IEnumerable<IDomainEvent> GetInTranEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                return e.Mode.Format == DomainEventFormat.InTran;
            };

            return GetDomainEvents().Where(isSatisfy);
        }

        private IEnumerable<IDomainEvent> GetOutTranEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                return e.Mode.Format == DomainEventFormat.OutTran;
            };

            return GetDomainEvents().Where(isSatisfy);
        }

        private IEnumerable<IDomainEvent> GetOutTranAsyncEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                return e.Mode.Format == DomainEventFormat.ByAsync;
            };

            return GetDomainEvents().Where(isSatisfy);
        }

        private IEnumerable<IDomainEvent> GetExceptionEvents()
        {
            Func<IDomainEvent, bool> isSatisfy = e =>
            {
                return e.Mode.Format == DomainEventFormat.AtException;
            };

            return GetDomainEvents().Where(isSatisfy);
        }

        private void Remove(params IDomainEvent[] domainEvents)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                _domainEvents.Remove(domainEvent);
            }
        }

        protected virtual IDomainEventAuthor[] GetTargetDomainEventAuthors()
        {
            return DomainModelTracker.GetAll().Where(x => x.Model is IDomainEventAuthor).Select(x => x.Model as IDomainEventAuthor).ToArray();
        }

        protected abstract IDomainEventScope InTranEventScope();
        protected abstract IDomainEventScope OutTranEventScope();
        protected abstract IDomainEventScope ExceptionEventScope();
    }
}
