using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;
using DomainShell.Tests.Infrastructure.Services;

namespace DomainShell.Tests.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository        
    {
        public PersonRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _personDao = new PersonDao(session);
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

        private List<Func<PersonProxy, bool>> GetSpecFuncList(ISelectionSpec<PersonPredicate> spec)
        {
            List<Func<PersonProxy, bool>> list = new List<Func<PersonProxy, bool>>();

            foreach (KeyValuePair<PersonPredicateItem, object> keyValue in spec.Predicate())
            {
                Func<PersonProxy, bool> func = person =>
                {
                    if (keyValue.Key == PersonPredicateItem.LikeName)
                    {
                        return person.Name != null && person.Name.StartsWith(keyValue.Value.ToString());
                    }
                    else if (keyValue.Key == PersonPredicateItem.City)
                    {
                        return person.Address.City != null && person.Address.City == keyValue.Value.ToString();
                    }

                    return true;
                };

                list.Add(func);
            }

            //if (spec.Predicate().And && !specFuncList.All(x => x(person)))
            //{
            //    continue;
            //}
            //else if (!spec.Predicate().And && !specFuncList.Any(x => x(person)))
            //{
            //    continue;
            //}

            return list;
        }

        public IEnumerable<PersonEntity> List(ISelectionSpec<PersonPredicate> spec)
        {
            List<Func<PersonProxy, bool>> specFuncList = GetSpecFuncList(spec);

            IEnumerable<PersonDto> dtos = _personDao.GetList(spec.Predicate());

            foreach (PersonDto dto in dtos)
            {
                PersonProxy person = new PersonProxy(dto); 

                yield return person;
            }   
        }

        public void Save(PersonEntity aggregateRoot)
        {
            PersonProxy person = aggregateRoot as PersonProxy;            

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
        }
    }
}
