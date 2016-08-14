using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IService<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
    }
}
