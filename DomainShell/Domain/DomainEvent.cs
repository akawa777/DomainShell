using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IDomainEvent
    {

    }

    public interface IDomainEventCollection
    {
        IEnumerable<IDomainEvent> GetEvents();
        void ClearEvents();
    }

    public interface IDomianEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        void Handle(TDomainEvent domainEvent);
    }
}
