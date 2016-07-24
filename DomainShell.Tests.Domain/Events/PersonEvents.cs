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
    public class PersonAddedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public class PersonUpdatedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public class PersonRemovedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    public class PersonEventHandler : 
        IDomainEventHandler<PersonAddedEvent, bool>,
        IDomainEventHandler<PersonUpdatedEvent, bool>,        
        IDomainEventHandler<PersonRemovedEvent, bool>
    {
        protected PersonWriteRepository _repository = new PersonWriteRepository();        

        protected bool Validate(Person person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                return false;
            }

            return true;
        }

        public bool Handle(PersonAddedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!Validate(person))
            {
                return false;
            }

            _repository.Add(person);

            return true;
        }

        public bool Handle(PersonUpdatedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!Validate(person))
            {
                return false;
            }

            _repository.Update(person);

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
