using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;
using DomainShell.Tests.Infrastructure.Daos;

namespace DomainShell.Tests.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository        
    {
        public PersonRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _personDao = new PersonDao(session.GetPort<System.Data.Common.DbConnection>());
        }

        private IDomainEventDispatcher _domainEventDispatcher;
        private PersonDao _personDao;

        public PersonEntity Find(PersonId id)
        {
            PersonDto dto = _personDao.Find(id.Value);

            if (dto == null)
            {
                return null;
            }

            PersonProxy person = new PersonProxy(dto);

            return person;
        }

        public IEnumerable<PersonEntity> List(ISelectionSpec<PersonPredicate> spec)
        {
            PersonDao.Predicate predicate = new PersonDao.Predicate();
            predicate.And = spec.Predicate().And;

            foreach (KeyValuePair<PersonPredicateItem, object> keyValue in spec.Predicate())
            {
                if (keyValue.Key == PersonPredicateItem.LikeName)
                {
                    predicate[PersonDao.PredicateItem.LikeName] = keyValue.Value;
                }

                if (keyValue.Key == PersonPredicateItem.City)
                {
                    predicate[PersonDao.PredicateItem.City] = keyValue.Value;
                }
            }

            IEnumerable<PersonDto> dtos = _personDao.GetList(predicate);

            foreach (PersonDto dto in dtos)
            {
                PersonProxy person = new PersonProxy(dto); 

                yield return person;
            }   
        }

        public void Save(PersonEntity aggregateRoot)
        {
            PersonProxy person = aggregateRoot as PersonProxy;

            if (person.Transient && person.Deleted)
            {
                return;
            }

            if (!person.OnceVerified)
            {
                throw new Exception("not verified");
            }

            person.RewriteMemento();

            if (person.Transient && !person.Deleted)
            {
                _personDao.Insert(person.Memento);

                person.Transient = false;
            }
            else if (!person.Deleted)
            {
                _personDao.Update(person.Memento);
            }
            else if (person.Deleted)
            {
                _personDao.Delete(person.Memento);
            }

            foreach (IDomainEvent domainEvent in person.GetEvents())
            {
                _domainEventDispatcher.Dispatch(domainEvent);
            }

            person.ClearEvents();

            person.OnceVerified = false;
        }
    }
}
