using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Domain.Common
{
    public interface IIdService
    {
        string CreateId<TEntity>();
    }
}
