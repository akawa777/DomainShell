using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Extension;

namespace DomainShell.Tests.Domain.Infrastructure
{
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            _connection = DomainShell.Tests.Domain.Infrastructure.DataStore.CreateConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        private System.Data.Common.DbConnection _connection;
        private System.Data.Common.DbTransaction _transaction;
        private bool _complete = false;

        public object Session()
        {
            return _connection;
        }

        public void Complete()
        {
            _transaction.Commit();
            _complete = true;
        }

        public void Dispose()
        {
            if (!_complete)
            {
                _transaction.Rollback();
            }
        }
    }
}
