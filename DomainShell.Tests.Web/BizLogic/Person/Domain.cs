using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Web.BizLogic.Person
{
    public class Person : IAggregateRoot
    {
        private EventList _eventList = new EventList();

        public EventList EventList
        {
            get { return _eventList; }
        }

        public int Id { get; set; }
        public string Name { get; set; }

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

        public void Remove()
        {
            PersonRemovedEvent @event = new PersonRemovedEvent();

            @event.Person = this;

            _eventList.Callback(@event, x => 
            {
                if (!x) throw new Exception();
            });

            _eventList.Add(@event);
        }
    }

    public class PersonValidator
    {
        public bool Validate(Person person)
        {
            if (person != null && person.Id > 0 && person.Name != string.Empty)
            {
                return true;
            }

            return false;
        }
    }
}
