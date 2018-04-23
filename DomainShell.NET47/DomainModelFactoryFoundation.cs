using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{
    public abstract class DomainModelFactoryFoundationBase : IDomainModelFactory
    {
        public virtual T Create<T>() where T : class
        {
            if (!TryCreate<T>(out T model) || model == null)
            {
                model = new ProxyObject<T>().Material;
            }

            return model;
        }

        protected abstract bool TryCreate<T>(out T model) where T : class;
    }

    public static class DomainModelFactory
    {
        private static Func<IDomainModelFactory> _getDomainModelFactory;

        public static void Startup(Func<IDomainModelFactory> getDomainModelFactory)
        {
            _getDomainModelFactory = getDomainModelFactory;
        }

        private static void Validate()
        {
            if (_getDomainModelFactory == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static T Create<T>() where T : class
        {
            Validate();

            var domainModelFactory = _getDomainModelFactory();
            return domainModelFactory.Create<T>();
        }

        public static ProxyObject<T> CreateProxy<T>() where T : class
        {
            Validate();

            var domainModelFactory = _getDomainModelFactory();
            var model = domainModelFactory.Create<T>();

            return new ProxyObject<T>(model);
        }
    }
}
