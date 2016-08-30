using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Infrastructure
{
    public interface IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        void Save(TAggregateRoot aggregateRoot);
    }
}
