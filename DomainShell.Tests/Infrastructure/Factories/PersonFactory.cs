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
using DomainShell.Tests.Infrastructure.Services;

namespace DomainShell.Tests.Infrastructure.Factories
{
    public class PersonFactory : IPersonFactory
    {
        public PersonFactory(ISession session)
        {
            _idDao = new IdDao(session.GetPort<System.Data.Common.DbConnection>());
        }

        private IdDao _idDao;

        public PersonEntity Create(ICreationSpec<PersonEntity, PersonConstructorParameters> spec)
        {
            PersonProxy person = new PersonProxy(new PersonDto { PersonId = _idDao.Generate("Person") });
            person.Transient = true;

            spec.Satisfied(person);

            return person;
        }
    }
}
