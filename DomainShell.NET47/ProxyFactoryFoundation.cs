using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{
    public abstract class DomainModelProxyFactoryFoundationBase : IDomainModelProxyFactory
    {
        public virtual T Create<T>() where T : class
        {
            T proxy = CreateProxy<T>();

            if (proxy == null)
            {
                proxy = new ProxyObject<T>().Material;
            }

            return proxy;

        }

        protected abstract T CreateProxy<T>() where T : class;
    }

    public static class DomainModelProxyFactory
    {
        private static Func<IDomainModelProxyFactory> _getDomainModelProxyFactory;

        public static void Startup(Func<IDomainModelProxyFactory> getDomainModelProxyFactory)
        {            
            _getDomainModelProxyFactory = getDomainModelProxyFactory;
        }

        private static void Validate()
        {
            if (_getDomainModelProxyFactory == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static T Create<T>() where T : class
        {
            Validate();

            IDomainModelProxyFactory domainModelProxyFactory = _getDomainModelProxyFactory();
            return domainModelProxyFactory.Create<T>();
        }
    }
}
