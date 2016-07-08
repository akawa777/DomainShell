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
        public PersonEventHandler(PersonWriteRepository repository)
        {
            _repository = repository;
        }

        private PersonWriteRepository _repository;

        public void Handle(PersonAddedEvent @event)
        {
            _repository.Add(@event.AggregateRoot as Person);
        }

        public void Handle(PersonUpdatedEvent @event)
        {
            _repository.Update(@event.AggregateRoot as Person);
        }

        public bool Handle(PersonRemovedEvent @event)
        {
            _repository.Delete(@event.AggregateRoot as Person);

            return true;
        }
    }
}
