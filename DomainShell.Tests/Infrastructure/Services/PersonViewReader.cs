using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Infrastructure.Contracts;

namespace DomainShell.Tests.Infrastructure.Services
{
    public class PersonViewReader : IPersonViewReader
    {
        public PersonViewReader(ISession session)
        {
            _session = session;
        }

        private ISession _session;
        private PersonSql _personSql = new PersonSql();

        public IEnumerable<PersonViewDto> GetPersonViewList()
        {
            return Enumerable.Empty<PersonViewDto>();
        }
    }
}
