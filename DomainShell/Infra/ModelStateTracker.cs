using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Kernels;

namespace DomainShell.Infra
{
    public static class ModelStateTracker
    {
        private static Func<IModelStateTrackerKernel> _getKernel;

        public static void Startup(Func<IModelStateTrackerKernel> getKernel)
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

        public static void Mark(object domainModel)
        {
            Validate();

            var kernel = _getKernel();

            kernel.Mark(domainModel);
        }

        public static void Commit(object domainModel)
        {
            Validate();

            var kernel = _getKernel();

            kernel.Commit(domainModel);
        }

        internal static IModelStateTrackerKernel Current => _getKernel();
    }
}