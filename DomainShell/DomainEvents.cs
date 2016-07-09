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
            Raise(@event, true);    
        }        

        public static TResult Raise<TResult>(DomainEvent<TResult> @event)
        {
            return (TResult)Raise(@event, false);           
        }

        private static dynamic Raise(IDomainEvent @event, bool isVoid)
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

                if (isVoid)
                {
                    dynamicHandler.Handle(@event as dynamic);
                    result = null;
                }
                else
                {
                    result = dynamicHandler.Handle(@event as dynamic);
                }

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

            return result;

        }
    }    
}
