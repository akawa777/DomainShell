using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DomainShell.Kernels;

namespace DomainShell.App
{
    public interface IOpenScope : IDisposable
    {
        
    }

    public interface ITranScope : IDisposable
    {
        void Complete();
    }

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

        public static void OnException(Exception exception)
        {
            Validate();

            var session = _getKernel();
            session.OnException(exception);
        }
    }
}