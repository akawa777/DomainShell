using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace DomainShell.Tests.Infrastructure
{
    public class Session : IDisposable
    {
        public static void Config(Func<DbConnection> createConnection, Func<DbCommand, DbDataAdapter> createDataAdapter)
        {            
            _createConnection = createConnection;            
            _createDataAdapter = createDataAdapter;
        }

        private static Func<DbConnection> _createConnection;
        private static Func<DbCommand, DbDataAdapter> _createDataAdapter;
        private DbConnection _connection;
        
        public Session Open()
        {
            _connection = _createConnection();
            _connection.Open();

            return this;
        }

        public Transaction BegingTran()
        {
            if (_connection == null ||  _connection.State != ConnectionState.Open)
            {
                Open();
            }

            return new Transaction(_connection.BeginTransaction());
        }

        public void Dispose()
        {
            _connection.Dispose();
            _connection = null;
        }

        public DbDataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            return _createDataAdapter(selectCommand);
        }

        public DbCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }
    }

    public class Transaction : IDisposable
    {
        public Transaction(DbTransaction tran)
        {
            _tran = tran;
        }

        public Transaction(DbTransaction tran, DbConnection connection)
            : this(tran)
        {
            _connection = connection;
        }

        private DbConnection _connection;
        private DbTransaction _tran;

        public void Commit()
        {
            _tran.Commit();
        }

        public void Dispose()
        {
            _tran.Dispose();

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
