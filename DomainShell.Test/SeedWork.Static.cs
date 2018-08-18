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
        private static Func<ISessionKernel> _getKernel;

        public static void Startup(Func<ISessionKernel> getKernel)
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

        public static IOpenScope Open()
        {
            Validate();

            var session = _getKernel();
            return session.Open();
        }

        public static ITranScope Tran()
        {
            Validate();

            var session = _getKernel();
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

    public static class SessionExceptionCatcher
    {
        private static Func<ISessionExceptionCatcherKernel> _getKernel;

        public static void Startup(Func<ISessionExceptionCatcherKernel> getKernel)
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

        public static void Catch(Exception exception)
        {
            Validate();

            var kernel = _getKernel();

            kernel.Catch(exception);
        }
    }

    public static class Log
    {
        private static Func<ILogKernel> _getKernel;

        public static void Startup(Func<ILogKernel> getKernel)
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

        public static string[] Messages
        {
            get
            {
                Validate();

                var kernel = _getKernel();

                return kernel.Messages;
            }
        }

        public static void SetMessage(string message)
        {
            Validate();

            var kernel = _getKernel();

            kernel.SetMessage(message);
        }
    }
}