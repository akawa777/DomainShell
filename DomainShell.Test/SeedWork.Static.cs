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