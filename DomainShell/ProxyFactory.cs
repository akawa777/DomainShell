using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{
    public interface IDomainModelProxy
    {
        Type GetImplementType();
    }
    
    public interface IDomainModelProxyFactory
    {
        T Create<T>() where T : class;
    }

    public static class DomainModelProxyFactory
    {
        private static Func<IDomainModelProxyFactory> _getDomainModelProxyFactory;

        public static void Startup(Func<IDomainModelProxyFactory> getDomainModelProxyFactoryPerThread)
        {            
            _getDomainModelProxyFactory = getDomainModelProxyFactoryPerThread;
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
