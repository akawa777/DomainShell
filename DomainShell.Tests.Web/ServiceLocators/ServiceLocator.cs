using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleInjector;
using System.Reflection;

namespace DomainShell.Tests.Web.ServiceLocators
{
    public interface IServiceLocator
    {
        
    }

    public class ServiceLocatorProvider
    {
        public void TargetAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies.ToList();
        }

        private List<Assembly> _assemblies = new List<Assembly>();

        public void EachServiceLocatorTypes(Action<Type> action)
        {
            List<Assembly> assemblies = _assemblies.ToList();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            if (!assemblies.Any(x => x.FullName == executingAssembly.FullName))
            {
                assemblies.Add(executingAssembly);
            }

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.GetInterface(typeof(IServiceLocator).FullName) == null || !type.IsClass)
                    {
                        continue;
                    }

                    action(type);
                }
            }
        }

    }
}