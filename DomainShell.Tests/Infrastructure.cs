using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DomainShell.Infrastructure;

namespace DomainShell.Tests
{
    public class DomainEventBundle : IDomainEventBundle
    {
        public void Bundle(IDomainEventRegister register)
        {
            register.Set<PersonAddedEvent>(() => new PersonEventHandler());
            register.Set<PersonUpdatedEvent>(() => new PersonEventHandler());
            register.Set<PersonRemovedEvent>(() => new PersonEventHandler());
        }
    }
}
