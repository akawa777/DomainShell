using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class IdProvider
    {
        public IdProvider(ISession session)
        {
            _session = session;
        }

        private ISession _session;

        public int Generate(string tableName)
        {
            return 1;
        }
    }
}
