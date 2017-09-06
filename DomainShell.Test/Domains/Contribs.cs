using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;

namespace DomainShell.Test.Domains
{
    public class DomainModelProxyFactoryImpl : IDomainModelProxyFactory
    {
        public DomainModelProxyFactoryImpl(Container container)
        {
            _container = container;
        }

        private Container _container;

        public T Create<T>() where T : class
        {
            return _container.GetInstance<T>();
        }
    }

    public class DomainModelTrackerFoundation : DomainModelTrackerFoundationBase
    {
        protected override object CreateStamp(object domainModel)
        {
            if (domainModel is IAggregateRoot model)
            {
                return model.RecordVersion;
            }

            return null;
        }
    }

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
    

    public class CurrentConnection : IConnection, ICurrentConnection
    {
        public CurrentConnection(IDbConnection connection, UnitOfWork unitOfWork)
        {
            _connection = connection;
            _unitOfWork = unitOfWork;
        }

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private UnitOfWork _unitOfWork;

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

        public void BeginCommit()
        {            
            _unitOfWork.Save();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void DisposeTran(bool completed)
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
        public SessionFoundation(IConnection connection)
        {

            _connection = connection;
        }

        private IConnection _connection;
        

        protected override IConnection GetConnection()
        {
            return _connection;
        }
    }

    public class UnitOfWork
    {
        public UnitOfWork(Container container)
        {
            _container = container;
        }

        private Container _container;

        public void Save()
        {
            IAggregateRoot[] domainModels = GetTargetDomainModels();

            foreach (IAggregateRoot domainModel in domainModels)
            {
                Save(domainModel);
            }
        }

        private IAggregateRoot[] GetTargetDomainModels()
        {
            IAggregateRoot[] deletedModels = GetDeletedDomainModels();
            IAggregateRoot[] modifiedModels = GetModifiedDomainModels();

            return deletedModels.Concat(modifiedModels).ToArray();
        }

        private IAggregateRoot[] GetDeletedDomainModels()
        {
            Func<TrackPack, bool> deleted = x =>
            {
                return x.Model is IAggregateRoot model && model.Dirty.Is && model.Deleted;
            };

            return DomainModelTracker.GetAll().Where(deleted).Select(x => x.Model as IAggregateRoot).ToArray();
        }

        private IAggregateRoot[] GetModifiedDomainModels()
        {
            Func<TrackPack, bool> modified = x =>
            {
                return x.Model is IAggregateRoot model && model.Dirty.Is && !model.Deleted;
            };

            return DomainModelTracker.GetAll().Where(modified).Select(x => x.Model as IAggregateRoot).ToArray();
        }

        private void Save(IAggregateRoot domainModel)
        {
            Type domainModelType;

            if (domainModel is IDomainModelProxy proxy) domainModelType = proxy.GetImplementType();
            else domainModelType = domainModel.GetType();

            Type writeRepositoryType = typeof(IWriteRepository<>).MakeGenericType(domainModelType);
            object writeRepository = _container.GetInstance(writeRepositoryType);
            MethodInfo method = writeRepositoryType.GetMethod("Save", new Type[] { domainModelType });
            method.Invoke(writeRepository, new object[] { domainModel });
        }        
    }    
}