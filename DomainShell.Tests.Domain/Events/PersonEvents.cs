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
    internal class PersonAddedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    internal class PersonUpdatedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    internal class PersonRemovedEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
    }

    internal class PersonEventHandler : 
        IDomainEventHandler<PersonAddedEvent, bool>,
        IDomainEventHandler<PersonUpdatedEvent, bool>,        
        IDomainEventHandler<PersonRemovedEvent, bool>
    {
        protected PersonWriteRepository _repository = new PersonWriteRepository();        

        protected bool Validate(PersonModel person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                return false;
            }

            return true;
        }

        public bool Handle(PersonAddedEvent @event)
        {
            PersonModel person = @event.AggregateRoot as PersonModel;

            if (!Validate(person))
            {
                return false;
            }

            _repository.Add(person);

            return true;
        }

        public bool Handle(PersonUpdatedEvent @event)
        {
            PersonModel person = @event.AggregateRoot as PersonModel;

            if (!Validate(person))
            {
                return false;
            }

            _repository.Update(person);

            return true;
        }

        public bool Handle(PersonRemovedEvent @event)
        {
            PersonModel person = @event.AggregateRoot as PersonModel;

            _repository.Delete(person);

            return true;
        }
    }
}
