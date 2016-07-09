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
    public class PersonTranUpdatedEvent : DomainEvent<bool>
    {
        public DbConnection Connection { get; set; }
    }

    public class PersonTranUpdatedEventHandler :  
        PersonEventHandler,
        IDomainEventHandler<PersonTranUpdatedEvent, bool>
    {
        public bool Handle(PersonTranUpdatedEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            if (!Validate(person))
            {
                return false;
            }

            _repository.Update(person, @event.Connection);

            return true;
        }
    }
}
