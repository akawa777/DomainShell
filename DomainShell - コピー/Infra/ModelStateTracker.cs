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
    public interface IModelStateTrack
    {
        object DomainModel { get; }
        object Tag { get; }
        void Commit();
        bool Comiited { get; }
    }

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

        public static IModelStateTrack Get(object domainModel)
        {
            Validate();

            var kernel = _getKernel();

            return kernel.Get(domainModel);
        }

        public static IEnumerable<IModelStateTrack> GetAll()
        {
            Validate();

            var kernel = _getKernel();

            return kernel.GetAll();
        }

        public static void Revoke(object domainModel)
        {
            Validate();

            var kernel = _getKernel();

            kernel.Revoke(domainModel);
        }

        public static void RevokeAll()
        {
            Validate();

            var kernel = _getKernel();

            kernel.RevokeAll();
        }

        internal static IModelStateTrackerKernel Current => _getKernel();
    }
}