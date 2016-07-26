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
    public class PersonUpdatedInTranEvent : IDomainEvent<bool>
    {
        public IAggregateRoot AggregateRoot { get; set; }
        public object Session { get; set; }
    }

    public class PersonUpdatedInTranEventHandler :
        PersonEventHandler,
        IDomainEventHandler<PersonUpdatedInTranEvent, bool>
    {
        public bool Handle(PersonUpdatedInTranEvent @event)
        {
            PersonModel person = @event.AggregateRoot as PersonModel;

            _repository.Update(person, @event.Session);

            return true;
        }
    }
}
