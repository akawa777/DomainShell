﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.Common;
using DomainShell.Infrastructure;

namespace DomainShell.Tests
{
    public class SqliteSessionKernel : ISessionKernel<DbConnection>
    {
        public static void Config(Func<DbConnection> createConnection)
        {
            _createConnection = createConnection;
        }

        private static Func<DbConnection> _createConnection;

        public SqliteSessionKernel()
        {
            _connection = _createConnection();
        }

        private DbConnection _connection;
        private DbTransaction _transaction;

        public DbConnection GetConnectionPort()
        {
            return _connection;
        }

        public void Open()
        {
            _connection.Open();
        }

        public void Close()
        {
            _connection.Close();
        }

        public void BeginTran()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
