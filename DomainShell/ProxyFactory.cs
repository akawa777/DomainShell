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

        public static void Startup(Func<IDomainModelProxyFactory> getDomainModelProxyFactoryByCurrentThread)
        {
            _getDomainModelProxyFactory = getDomainModelProxyFactoryByCurrentThread;
        }

        public static T Create<T>() where T : class
        {
            IDomainModelProxyFactory domainModelProxyFactory = _getDomainModelProxyFactory();
            return domainModelProxyFactory.Create<T>();
        }
    }
}
