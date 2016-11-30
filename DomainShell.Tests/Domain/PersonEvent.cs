using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain
{
    public class PersonDeletedEvent : IDomainEvent
    {
        public string PersonId { get; set; }
        public string PersonName { get; set; }
        public string Email { get; set; }
    }
}
