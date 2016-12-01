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
        public PersonFactory(IPersonIdGenerator personIdGenerator)
        {
            _personIdGenerator = personIdGenerator;
        }

        private IPersonIdGenerator _personIdGenerator;

        public PersonEntity Create(ICreationSpec<PersonEntity, PersonConstructorParameters> spec)
        {
            PersonProxy person = new PersonProxy(new PersonDto { PersonId = _personIdGenerator.Generate() });
            person.Transient = true;

            spec.Satisfied(person);

            return person;
        }
    }
}
