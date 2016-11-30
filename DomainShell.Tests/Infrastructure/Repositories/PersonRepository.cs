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
        }

        private IDomainEventDispatcher _domainEventDispatcher;
        private List<PersonDto> _memoryStore = new List<PersonDto>();        
        private PersonSql _personSql = new PersonSql();

        public PersonEntity Find(PersonId id)
        {
            PersonDto memento = _memoryStore.FirstOrDefault(x => x.PersonId == id.Value);

            if (memento == null)
            {
                return null;
            }

            PersonProxy person = new PersonProxy(memento);

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

            return list;
        }

        public IEnumerable<PersonEntity> List(ISelectionSpec<PersonPredicate> spec)
        {
            List<Func<PersonProxy, bool>> specFuncList = GetSpecFuncList(spec);

            foreach (PersonDto memento in _memoryStore)
            {
                PersonProxy person = new PersonProxy(memento);                

                if (spec.Predicate().And && !specFuncList.All(x => x(person)))
                {
                    continue;
                }
                else if (!spec.Predicate().And && !specFuncList.Any(x => x(person)))
                {
                    continue;
                }

                yield return person;
            }   
        }

        public void Save(PersonEntity aggregateRoot)
        {
            PersonProxy person = aggregateRoot as PersonProxy;            

            person.RewriteMemento();            

            if (person.Transient && !person.Deleted)
            {
                _memoryStore.Add(person.Memento);

                person.Transient = false;
            }
            else if (!person.Deleted)
            {
                
            }
            else if (person.Deleted)
            {
                _memoryStore.Remove(person.Memento);                
            }

            foreach (IDomainEvent domainEvent in person.GetEvents())
            {
                _domainEventDispatcher.Dispatch(domainEvent);
            }

            person.ClearEvents();
        }
    }
}
