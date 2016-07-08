using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell;
using DomainShell.Tests.Web.Events;

namespace DomainShell.Tests.Web.Models
{
    public class Person : IAggregateRoot
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public bool Add()
        {
            PersonAddedEvent @event = new PersonAddedEvent();

            @event.AggregateRoot = this;

            return DomainEvents.Raise(@event);
        }

        public bool Update()
        {
            PersonUpdatedEvent @event = new PersonUpdatedEvent();

            @event.AggregateRoot = this;

            return DomainEvents.Raise(@event);
        }

        public bool Remove()
        {
            PersonRemovedEvent @event = new PersonRemovedEvent();

            @event.AggregateRoot = this;

            return DomainEvents.Raise(@event);
        }
    }
}
