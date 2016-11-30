using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Infrastructure.Factories
{
    public class PersonFactory : IPersonFactory
    {
        public PersonEntity Create(ICreationSpec<PersonEntity, PersonOpions> spec)
        {
            PersonProxy person = new PersonProxy(new PersonDto { PersonId = spec.Options().PersonId, HistoryList = new List<HistoryDto>() });
            person.Transient = true;

            spec.Satisfied(person);

            return person;
        }
    }
}
