using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;

namespace DomainShell.Test
{
    public static class Session
    {
        private static Func<ISessionKernel> _getSession;

        public static void Startup(Func<ISessionKernel> getSession)
        {
            _getSession = getSession;
        }

        private static void Validate()
        {
            if (_getSession == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static IOpenScope Open()
        {
            Validate();

            var session = _getSession();
            return session.Open();
        }

        public static ITranScope Tran()
        {
            Validate();

            var session = _getSession();
            return session.Tran();
        }
    }

    public static class DomainEventPublisher
    {
        private static Func<IDomainEventPublisherKernel<IAggregateRoot>> _getKernel;

        public static void Startup(Func<IDomainEventPublisherKernel<IAggregateRoot>> getKernel)
        {
            _getKernel = getKernel;
        }

        private static void Validate()
        {
            if (_getKernel == null)
            {
                throw new InvalidOperationException("StratUp not runninng.");
            }
        }

        public static void Publish(IAggregateRoot aggregateRoot)
        {
            Validate();

            var kernel = _getKernel();

            kernel.Publish(aggregateRoot);
        }
    }
}