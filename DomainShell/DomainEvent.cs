using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEventAuthor<TDomainEvent>
    {
        IEnumerable<TDomainEvent> GetEvents();
        void ClearEvents();
    }
}
