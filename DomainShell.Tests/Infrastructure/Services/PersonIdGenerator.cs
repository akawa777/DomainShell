using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Contracts;

namespace DomainShell.Tests.Infrastructure.Services
{
    public class PersonIdGenerator : IPersonIdGenerator
    {
        public PersonIdGenerator(ISession session)
        {
            _session = session;
            _idGenerator = new IdGenerator(_session);
        }

        private ISession _session;

        private IdGenerator _idGenerator;

        public string Generate()
        {
            return _idGenerator.Generate("Person");
        }
    }
}
