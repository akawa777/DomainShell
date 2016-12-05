using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Infrastructure.Contracts;
using DomainShell.Tests.Infrastructure.Daos;

namespace DomainShell.Tests.Infrastructure.Services
{
    public class PersonViewReader : IPersonViewReader
    {
        public PersonViewReader(ISession session)
        {
            _personDao = new PersonDao(session.GetPort<System.Data.Common.DbConnection>());
        }
        
        private PersonDao _personDao;

        public IEnumerable<PersonViewDto> GetPersonViewList()
        {
            return _personDao.GetViewList();
        }
    }
}
