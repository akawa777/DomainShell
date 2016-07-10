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
    public class PersonUpdatedInTranEvent : DomainEvent<bool>
    {
        public DbConnection Connection { get; set; }
    }

    public class PersonUpdatedInTranEventHandler :
        PersonEventHandler,
        IDomainEventHandler<PersonUpdatedInTranEvent, bool>
    {
        public bool Handle(PersonUpdatedInTranEvent @event)
        {
            Person person = @event.AggregateRoot as Person;

            _repository.Update(person, @event.Connection);

            return true;
        }
    }
}
