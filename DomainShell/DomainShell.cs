using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Base;
using DomainShell.Infrastructure;

namespace DomainShell
{
    public interface IAggregateRoot
    {
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public abstract class DomainEvent<TResult> : IDomainEvent
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public interface IDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : DomainEvent
    {        
        void Handle(TEvent @event);
    }

    public interface IDomainEventHandler<TEvent, TResult> : IDomainEventHandler where TEvent : DomainEvent<TResult>
    {        
        TResult Handle(TEvent @event);
    }    

    public class DomainEvents
    {
        private static DomainEventContainer _container = new DomainEventContainer();

        private static Dictionary<string, bool> _assemblyMap = new Dictionary<string, bool>();
        
        private static void Bundle(IDomainEvent @event)
        {
            Assembly assembly = @event.GetType().Assembly;

            if (_assemblyMap.ContainsKey(assembly.FullName))
            {
                return;
            }            

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterface(typeof(IDomainEventBundle).FullName) != null)
                {
                    IDomainEventBundle bundle = Activator.CreateInstance(type) as IDomainEventBundle;
                    bundle.Bundle(_container);
                }
            }

            _assemblyMap[assembly.FullName] = true;
        }        

        public static void Raise(DomainEvent @event)
        {
            Bundle(@event);

            IDomainEventHandler handler = _container.Load(@event);

            bool canAspect = handler is IDomainEventAspect;
            IDomainEventAspect aspect = handler as IDomainEventAspect;

            if (canAspect)
            {
                aspect.BeginEvent(@event, handler);
            }

            try
            {
                dynamic dynamicHandler = handler as dynamic;
                dynamicHandler.Handle(@event as dynamic);

                if (canAspect)
                {
                    aspect.SuccessEvent(@event, handler, null);
                }
            }
            catch(Exception e)
            {
                if (canAspect)
                {
                    aspect.FailEvent(@event, handler, e);
                }

                throw e;
            }            
        }

        public static TResult Raise<TResult>(DomainEvent<TResult> @event)
        {
            Bundle(@event);

            IDomainEventHandler handler = _container.Load(@event);

            bool canAspect = handler is IDomainEventAspect;
            IDomainEventAspect aspect = handler as IDomainEventAspect;

            if (canAspect)
            {
                aspect.BeginEvent(@event, handler);
            }

            dynamic result;

            try
            {
                dynamic dynamicHandler = handler as dynamic;
                result = dynamicHandler.Handle(@event as dynamic);

                if (canAspect)
                {
                    aspect.SuccessEvent(@event, handler, result);
                }
            }
            catch (Exception e)
            {
                if (canAspect)
                {
                    aspect.FailEvent(@event, handler, e);
                }

                throw e;
            }

            return (TResult)result;
        }
    }    
}
