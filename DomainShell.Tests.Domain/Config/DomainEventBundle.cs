using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DomainShell.Config;
using DomainShell.Tests.Domain.Repositories.Write;
using DomainShell.Tests.Domain.Events;
using DomainShell.Tests.Domain.Models;

namespace DomainShell.Tests.Domain.Config
{
    public class DomainEventBundle : IDomainEventBundle
    {
        public void Bundle(IDomainEventRegister register)
        {
            register.Set<PersonAddedEvent>(() => new PersonEventHandler());
            register.Set<PersonUpdatedEvent>(() => new PersonEventHandler());            
            register.Set<PersonRemovedEvent>(() => new PersonEventHandler());
            register.Set<PersonUpdatedInTranEvent>(() => new PersonUpdatedInTranEventHandler());
        }
    }
}
