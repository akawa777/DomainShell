using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using DomainShell.Kernels;

namespace DomainShell
{
    public static class DomainModelFactory
    {
        private static Func<IDomainModelFactoryKernel> _getDomainModelFactoryKernel;

        public static void Startup(Func<IDomainModelFactoryKernel> getDomainModelFactoryKernel)
        {
            _getDomainModelFactoryKernel = getDomainModelFactoryKernel;
        }

        private static void Validate()
        {
            if (_getDomainModelFactoryKernel == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static T Create<T>() where T : class
        {
            return Create(typeof(T)) as T;
        }

        public static object Create(Type type)
        {
            Validate();

            var domainModelFactory = _getDomainModelFactoryKernel();

            if (!domainModelFactory.TryCreate(type, out object model) || model == null)
            {
                model = CreateObject(type);
            }

            return model;
        }

        public static ProxyObject<T> CreateProxy<T>() where T : class
        {
            var model = Create<T>();

            return new ProxyObject<T>(model);
        }

        private static object CreateObject(Type type)
        {
            dynamic proxyObject = Activator.CreateInstance(typeof(ProxyObject<>).MakeGenericType(type));

            return proxyObject.Material;
        }
    }
}
