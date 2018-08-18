using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEventPublisherKernel<TAggregateRoot>
    {
        void Publish(TAggregateRoot aggregateRoot);
    }    
}
