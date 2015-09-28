using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Domain;

namespace DomainDesigner.Tests.DomainShell
{
    public class Person : IAggregateRoot
    {
        private EventList _eventList = new EventList();

        public EventList EventList
        {
            get { return _eventList; }
        }

        public int Id { get; set; }
        public int Name { get; set; }

        public void Add()
        {
            PersonAddedEvent @event = new PersonAddedEvent();

            @event.Person = this;

            _eventList.Add(@event);
        }

        public void Update()
        {
            PersonUpdatedEvent @event = new PersonUpdatedEvent();

            @event.Person = this;

            _eventList.Add(@event);
        }
    }
}
