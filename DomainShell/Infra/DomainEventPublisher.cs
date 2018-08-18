using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Kernels;

namespace DomainShell.Infra
{    
    public static class DomainEventPublisher
    {
        private static Func<IDomainEventPublisherKernel> _getKernel;

        public static void Startup(Func<IDomainEventPublisherKernel> getKernel)
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

        public static void Publish(object aggregateRoot)
        {
            Validate();

            var kernel = _getKernel();

            if (!aggregateRoot.GetType().IsSubclassOf(kernel.AggregateRootDefineType)
                && !aggregateRoot.GetType().GetInterfaces().Any(x => x == kernel.AggregateRootDefineType))
            {
                throw new InvalidOperationException($"{aggregateRoot.GetType()} is not inherited {kernel.AggregateRootDefineType}");
            }

            kernel.Publish(aggregateRoot);
        }
    }
}
