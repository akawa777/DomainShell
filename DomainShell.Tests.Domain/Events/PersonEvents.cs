using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Repositories.Write;
using DomainShell.Tests.Domain.Service;
using System.Data.Common;

namespace DomainShell.Tests.Domain.Events
{
    public class PersonAddedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonUpdatedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonTranUpdatedEvent : DomainEvent<bool>
    {
        public DbConnection Connection { get; set; }
    }

    public class PersonRemovedEvent : DomainEvent<bool>
    {
        
    }

    public class PersonEventHandler : 
        IDomainEventHandler<PersonAddedEvent, bool>,
        IDomainEventHandler<PersonUpdatedEvent, bool>,
        IDomainEventHandler<PersonTranUpdatedEvent, bool>, 
        IDomainEventHandler<PersonRemovedEvent, bool>
    {
        private PersonWriteRepository _repository = new PersonWriteRepository();
        private PersonValidator _validator = new PersonValidator();

        public bool Handle(PersonAddedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!_validator.Validate(person))
            {
                return false;
            }

            _repository.Add(person);

            return true;
        }

        public bool Handle(PersonUpdatedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!_validator.Validate(person))
            {
                return false;
            }

            _repository.Update(person);

            return true;
        }

        public bool Handle(PersonTranUpdatedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!_validator.Validate(person))
            {
                return false;
            }

            _repository.Update(person, @event.Connection);

            return true;
        }

        public bool Handle(PersonRemovedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            _repository.Delete(person);

            return true;
        }
    }
}
