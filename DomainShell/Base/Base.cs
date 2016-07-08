using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Base
{
    public interface IDomainEvent
    {

    }

    public interface IDomainEventHandler
    {

    }

    public interface IDomainEventAspect
    {
        void BeginEvent(IDomainEvent @event, IDomainEventHandler handler);
        void SuccessEvent(IDomainEvent @event, IDomainEventHandler handler, object result);
        void FailEvent(IDomainEvent @event, IDomainEventHandler handler, Exception exception);
    }
}
