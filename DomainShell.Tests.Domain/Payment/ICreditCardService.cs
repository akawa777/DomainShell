using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Payment
{
    public interface ICreditCardService<TAggregateRoot> 
        : IService<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        void Pay(TAggregateRoot aggregateRoot);
    }
}
