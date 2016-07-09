using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell;

namespace DomainShell.Tests
{
    public class PersonAddedEvent : DomainEvent
    {
        
    }

    public class PersonUpdatedEvent : DomainEvent
    {
        
    }

    public class PersonRemovedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonEventHandler : IDomainEventHandler<PersonAddedEvent>, IDomainEventHandler<PersonUpdatedEvent>, IDomainEventHandler<PersonRemovedEvent, bool>
    {
        public void Handle(PersonAddedEvent @event)
        {
            
        }

        public void Handle(PersonUpdatedEvent @event)
        {
            
        }

        public bool Handle(PersonRemovedEvent @event)
        {
            return true;
        }
    }
}
