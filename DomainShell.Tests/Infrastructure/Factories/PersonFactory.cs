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

namespace DomainShell.Tests.Infrastructure.Factories
{
    public class PersonFactory : IPersonFactory
    {
        public PersonFactory(ISession session)
        {
            _idGenerator = new IdGenerator(session);
        }

        private IdGenerator _idGenerator;

        public PersonEntity Create(ICreationSpec<PersonEntity, PersonConstructorParameters> spec)
        {
            PersonProxy person = new PersonProxy(new PersonDto { PersonId =  _idGenerator.Generate("Person") });
            person.Transient = true;

            spec.Satisfied(person);

            return person;
        }
    }
}
