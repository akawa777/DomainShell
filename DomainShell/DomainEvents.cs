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

        private static void Validate(IDomainEvent @event, Type returnType, IDomainEventHandler handler)
        {
            if (handler == null)
            {
                throw new Exception(string.Format("not registered {0} Handler class.", @event.GetType().ToString()));
            }

            MethodInfo method = handler.GetType().GetMethod("Handle", new Type[] { @event.GetType() });

            if (method == null || method.ReturnType != returnType)
            {
                throw new Exception(string.Format("not define Handle method for {0}. return type of Handle method must be {1}.", @event.GetType().ToString(), returnType.ToString()));
            }
        }

        private static object Raise(IDomainEvent @event, Type returnType)
        {
            Bundle(@event);

            IDomainEventHandler handler = _container.Load(@event);

            Validate(@event, returnType, handler);

            bool canAspect = handler is IDomainEventAspect;
            IDomainEventAspect aspect = handler as IDomainEventAspect;

            if (canAspect)
            {
                aspect.BeginEvent(@event, handler);
            }

            dynamic result;

            try
            {                
                MethodInfo method = handler.GetType().GetMethod("Handle", new Type[] { @event.GetType() });
                result = method.Invoke(handler, new object[] { @event });

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

        public static void Raise(IDomainEvent @event)
        {
            Raise(@event, typeof(void));
        }

        public static TResult Raise<TResult>(IDomainEvent<TResult> @event)
        {
            return (TResult)Raise(@event, typeof(TResult));
        }
    }    
}
