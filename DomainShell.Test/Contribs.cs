using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;

namespace DomainShell.Test
{
    public class DomainEventFoundation : DomainEventFoundationBase
    {
        public DomainEventFoundation(Container container)
        {
            _container = container;
        }

        private Container _container;

        protected override IDomainEventScope InTranEventScope()
        {
            return new InTranEventScope(_container);
        }

        protected override IDomainEventScope OutTranEventScope()
        {
            return new OutTranEventScope(_container);
        }

        protected override IDomainEventScope ExceptionEventScope()
        {
            return new ExceptionEventScope(_container);
        }        
    }

    public class InTranEventScope : IDomainEventScope
    {
        public InTranEventScope(Container container)
        {
            _container = container;
        }

        private Container _container;

        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
        }

        public void Dispose()
        {

        }
    }

    public class OutTranEventScope : IDomainEventScope
    {
        public OutTranEventScope(Container container)
        {
            _container = container;
            _scope = ThreadScopedLifestyle.BeginScope(_container);
        }

        private Container _container;
        private Scope _scope; 

        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }

    public class ExceptionEventScope : OutTranEventScope
    {
        public ExceptionEventScope(Container container) : base(container)
        {
            
        }
    }

    public interface IConnection : IDisposable
    {
        IDbCommand CreateCommand();
    }

    public class Connection : IConnection
    {
        public Connection(IDbConnection connection)
        {
            _connection = connection;

        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

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

        public void DisposeTran()
        {
            _transaction.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = _connection.CreateCommand();
            command.Transaction = _transaction;

            return command;
        }        
    }

    public class SessionFoundation : SessionFoundationBase
    {
        public SessionFoundation(Connection connection, Container container)
        {
            _connection = connection;
            _container = container;
        }

        private Connection _connection;
        private Container _container;

        protected override OpenScopeBase OpenScopeBase()
        {
            return new OpenScope(_connection);
        }

        protected override TranScopeBase TranScopeBase()
        {
            return new TranScope(_connection, _container);
        }
    }

    public class OpenScope : OpenScopeBase
    {
        public OpenScope(Connection connection)
        {
            _connection = connection;
        }

        private Connection _connection;

        public override void Open()
        {            
            _connection.Open();
        }

        protected override void Close()
        {
            _connection.Close();
        }
    }

    public class TranScope : TranScopeBase
    {
        public TranScope(Connection connection, Container container)
        {
            _connection = connection;
            _container =  container;
        }

        private Connection _connection;        
        private Container _container;
        
        public override void BeginTran()
        {
            _connection.BeginTran();
        }

        protected override void BeginCommit()
        {
            UnitOfWork unitOfWork = _container.GetInstance<UnitOfWork>();
            unitOfWork.Save();
        }

        protected override void Commit()
        {            
            _connection.Commit();
        }

        protected override void Dispose(bool completed)
        {
            _connection.DisposeTran();
        }
    }

    public class UnitOfWork : UnitOfWorkFoundationBase<IAggregateRoot>
    {
        public UnitOfWork(Container container)
        {
            _container = container;
        }

        private Container _container;
        
        protected override Dirty GetDirty(IAggregateRoot domainModel)
        {
            return domainModel.Dirty;
        }

        protected override Deleted GetDeleted(IAggregateRoot domainModel)
        {
            return domainModel.Deleted;
        }
        
        protected override IAggregateRoot[] GetTargetDomainModels()
        {
            return DomainModelTracker.GetAll().Where(x => x is IAggregateRoot).Select(x => x as IAggregateRoot).ToArray();
        }
        
        protected override void Save(IAggregateRoot domainModel)
        {
            Type writeRepositoryType = typeof(IWriteRepository<>).MakeGenericType(domainModel.GetType());
            object writeRepository = _container.GetInstance(writeRepositoryType);
            MethodInfo method = writeRepositoryType.GetMethod("Save", new Type[] { domainModel.GetType() });
            method.Invoke(writeRepository, new object[] { domainModel });
        }        
    }
}