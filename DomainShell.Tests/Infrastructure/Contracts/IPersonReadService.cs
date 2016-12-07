using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Infrastructure.Contracts
{
    public interface IPersonReadService
    {
        IEnumerable<PersonReadDto> GetPersonList();
    }
}
