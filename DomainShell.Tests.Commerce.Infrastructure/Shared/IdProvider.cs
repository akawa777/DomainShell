using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;

namespace DomainShell.Tests.Commerce.Infrastructure.Shared
{
    public class IdProvider
    {
        public IdProvider(ISession session)
        {
            _connection = session.GetPort<System.Data.Common.DbConnection>();
        }

        private System.Data.Common.DbConnection _connection;

        public int Generate(string tableName)
        {
            return 1;
        }
    }
}
