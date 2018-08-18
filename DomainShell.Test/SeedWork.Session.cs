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
        public SessionKernel(IDbConnection connection, IDomainEventCacheKernel<IDomainEvent> domainEventCache)
        {
            _connection = connection;
            _domainEventCache = domainEventCache;
        }
        
        private readonly IDbConnection _connection;        
        private readonly IDomainEventCacheKernel<IDomainEvent> _domainEventCache;        
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

    public class SessionExceptionCatcherKernel : SessionExceptionCatcherKernelBase<IDomainEvent>
    {
        public SessionExceptionCatcherKernel(Container container, IDomainEventCacheKernel<IDomainEvent> domainEventCache) : base(domainEventCache)
        {
            _container = container;
        }

        private readonly Container _container;

        protected override void OnException(Exception exception)
        {

        }

        protected override void HandleDomainEventsOnException(Exception exception, IDomainEvent[] domainEvents)
        {
            using (ThreadScopedLifestyle.BeginScope(Bootstrap.Container))
            {
                foreach (var domainEvent in domainEvents)
                {
                    var handlerType = typeof(IDomainEventExceptionHandler<>).MakeGenericType(domainEvent.GetType());

                    if (_container.GetRegistration(handlerType) != null)
                    {
                        var handler = _container.GetInstance(handlerType) as dynamic;
                        handler.Handle(domainEvent as dynamic);
                    }
                }
            }
        }
    }
}