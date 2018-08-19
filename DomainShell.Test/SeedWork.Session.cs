using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public class SessionKernel : SessionKernelBase, IConnection
    {
        public SessionKernel(IDbConnection connection)
        {
            _connection = connection;
        }
        
        private readonly IDbConnection _connection;                
        private IDbTransaction _transaction;

        protected override void BeginOpen()
        {
            _connection.Open();
        }
        protected override void BeginTran()
        {
            _transaction = _connection.BeginTransaction();
        }

        protected override void EndTran(bool completed)
        {
            if (completed)
            {
                _transaction.Commit();                
            }
            else
            {
                _transaction.Rollback();
            }
        }

        protected override void EndOpen()
        {
            if (_transaction != null) _transaction.Dispose();
            _transaction = null;

            _connection.Close();
        }

        public void Dispose()
        {                      
            _connection.Dispose();
        }

        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();

            if (_transaction != null) command.Transaction = _transaction;

            return command;
        }
    }
}