using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Repositories.Write;

namespace DomainShell.Tests.Web.Events
{
    public class PersonAddedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonUpdatedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonRemovedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonEventHandler : 
        IDomainEventHandler<PersonAddedEvent, bool>,
        IDomainEventHandler<PersonUpdatedEvent, bool>, 
        IDomainEventHandler<PersonRemovedEvent, bool>
    {
        public PersonEventHandler(PersonWriteRepository repository)
        {
            _repository = repository;
        }

        private PersonWriteRepository _repository;

        public bool Handle(PersonAddedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (string.IsNullOrEmpty(person.Name))
            {
                return false;
            }

            _repository.Add(person);

            return true;
        }

        public bool Handle(PersonUpdatedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (string.IsNullOrEmpty(person.Id))
            {
                return false;
            }

            if (string.IsNullOrEmpty(person.Name))
            {
                return false;
            }

            _repository.Update(person);

            return true;
        }

        public bool Handle(PersonRemovedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (string.IsNullOrEmpty(person.Id))
            {
                return false;
            }

            _repository.Delete(person);

            return true;
        }
    }
}
