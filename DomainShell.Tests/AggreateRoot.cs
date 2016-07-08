using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell;

namespace DomainShell.Tests
{
    public class Person : IAggregateRoot
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public void Add()
        {
            PersonAddedEvent @event = new PersonAddedEvent();

            @event.AggregateRoot = this;

            DomainEvents.Raise(@event);
        }

        public void Update()
        {
            PersonUpdatedEvent @event = new PersonUpdatedEvent();

            @event.AggregateRoot = this;

            DomainEvents.Raise(@event);
        }

        public bool Remove()
        {
            PersonRemovedEvent @event = new PersonRemovedEvent();

            @event.AggregateRoot = this;

            return DomainEvents.Raise(@event);
        }
    }
}
