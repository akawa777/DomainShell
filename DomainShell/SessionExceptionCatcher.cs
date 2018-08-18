using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Kernels;

namespace DomainShell
{
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
}
