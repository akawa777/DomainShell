using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Event;

namespace DomainDesigner.Tests.DomainShell
{
    public class PersonAddedEvent : IDomainEvent
    {
        public Person Person { get; set; }

        public bool InTransaction()
        {
            return true;
        }
    }

    public class PersonUpdatedEvent : IDomainEvent
    {
        public Person Person { get; set; }

        public bool InTransaction()
        {
            return true;
        }
    }

    public class PersonRemovedEvent : IDomainEvent<bool>
    {
        public Person Person { get; set; }

        public bool InTransaction()
        {
            return true;
        }
    }

    public class PersonEventHandler : IDomainEventHandler<PersonAddedEvent>, IDomainEventHandler<PersonUpdatedEvent>, IDomainEventHandler<PersonRemovedEvent>
    {
        public PersonEventHandler(PersonWriteRepository repository)
        {
            _repository = repository;
        }

        private PersonWriteRepository _repository;

        private EventResult _result = new EventResult();

        public EventResult EventResult
        {
            get { return _result; }
        }

        public void Handle(PersonAddedEvent domainEvent)
        {
            _repository.Add(domainEvent.Person);            
        }

        public void Handle(PersonUpdatedEvent domainEvent)
        {
            _repository.Update(domainEvent.Person);            
        }

        public void Handle(PersonRemovedEvent domainEvent)
        {
            _repository.Delete(domainEvent.Person);
            _result.Set(domainEvent, true);
        }
    }
}
