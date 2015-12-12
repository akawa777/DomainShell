using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleInjector;
using System.Reflection;

namespace DomainShell.Tests.Web.ServiceLocators
{
    public class ServiceLocatorProvider
    {
        public ServiceLocatorProvider()
        {
            Assemblies = new List<Assembly>();
            Assemblies.Add(Assembly.GetExecutingAssembly());
        }

        public List<Assembly> Assemblies { get; set; }

        public Type[] GetServiceLocatorTypes()
        {
            List<Type> types = new List<Type>();
            foreach (Assembly assembly in Assemblies)
            {
                foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.GetInterface(typeof(IServiceLocator).FullName) == null || !type.IsClass)
                    {
                        continue;
                    }

                    types.Add(type);
                }
            }

            return types.ToArray();
        }
    }
}