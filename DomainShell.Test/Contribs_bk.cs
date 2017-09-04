//using System;
//using System.Linq;
//using System.Collections.Generic;
//using DomainShell;
//using SimpleInjector;
//using SimpleInjector.Lifestyles;
//using System.Data;

//namespace DomainShell.Test
//{
//    public class DomainEventFoundation : DomainEventFoundationBase
//    {
//        public DomainEventFoundation(Container container)
//        {
//            _container = container;
//        }

//        private Container _container;

//        protected override IDomainEventScope InTranEventScope()
//        {
//            return new InTranEventScope(_container);
//        }

//        protected override IDomainEventScope OutTranEventScope()
//        {
//            return new OutTranEventScope(_container);
//        }

//        protected override IDomainEventScope ExceptionEventScope()
//        {
//            return new ExceptionEventScope(_container);
//        }        
//    }

//    public class InTranEventScope : IDomainEventScope
//    {
//        public InTranEventScope(Container container)
//        {
//            _container = container;
//        }

//        private Container _container;

//        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
//        {
//            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
//        }

//        public void Dispose()
//        {

//        }
//    }

//    public class OutTranEventScope : IDomainEventScope
//    {
//        public OutTranEventScope(Container container)
//        {
//            _container = container;
//            _scope = ThreadScopedLifestyle.BeginScope(_container);
//        }

//        private Container _container;
//        private Scope _scope; 

//        public IDomainEventHandler<TDomainEvent> GetHandler<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
//        {
//            return _container.GetInstance<IDomainEventHandler<TDomainEvent>>();
//        }

//        public void Dispose()
//        {
//            _scope.Dispose();
//        }
//    }

//    public class ExceptionEventScope : OutTranEventScope
//    {
//        public ExceptionEventScope(Container container) : base(container)
//        {
            
//        }
//    }

//    public interface IConnection : IDisposable
//    {
//        IDbCommand CreateCommand();
//    }

//    public class Connection : IConnection
//    {
//        public Connection(IDbConnection connection)
//        {
//            _connection = connection;

//        }

//        private IDbConnection _connection;
//        private IDbTransaction _transaction;

//        public void Open()
//        {
//            _connection.Open();
//        }

//        public void Close()
//        {
//            _connection.Close();
//        }

//        public void BeginTran()
//        {
//            _transaction = _connection.BeginTransaction();
//        }

//        public void Commit()
//        {
//            _transaction.Commit();
//        }

//        public void Rollback()
//        {
//            _transaction.Rollback();
//        }

//        public void DisposeTran()
//        {
//            _transaction.Dispose();
//            _transaction = null;
//        }

//        public void Dispose()
//        {
//            _connection.Dispose();
//        }

//        public IDbCommand CreateCommand()
//        {
//            IDbCommand command = _connection.CreateCommand();
//            command.Transaction = _transaction;

//            return command;
//        }        
//    }

//    public class SessionFoundation : SessionFoundationBase
//    {
//        public SessionFoundation(Connection connection, Container container)
//        {
//            _connection = connection;
//            _container = container;
//        }

//        private Connection _connection;
//        private Container _container;

//        protected override OpenScopeBase OpenScopeBase()
//        {
//            return new OpenScope(_connection);
//        }

//        protected override TranScopeBase TranScopeBase()
//        {
//            return new TranScope(_connection, _container);
//        }
//    }

//    public class OpenScope : OpenScopeBase
//    {
//        public OpenScope(Connection connection)
//        {
//            _connection = connection;
//        }

//        private Connection _connection;

//        public override void Open()
//        {            
//            _connection.Open();
//        }

//        protected override void Close()
//        {
//            _connection.Close();
//        }
//    }

//    public class TranScope : TranScopeBase
//    {
//        public TranScope(Connection connection, Container container)
//        {
//            _connection = connection;
//            _container =  container;
//        }

//        private Connection _connection;        
//        private Container _container;
        
//        public override void BeginTran()
//        {
//            _connection.BeginTran();
//        }

//        protected override void Commit()
//        {
//            Save();
//            _connection.Commit();
//        }

//        private void Save()
//        {
//            object[] domainModels = DomainModelTracker.GetAll().ToArray();

//            foreach (object domainModel in domainModels)
//            {
//                if (domainModel is IAggregateRoot root)
//                {
//                    ModelState state = GetAggregateRootState(root);

//                    Type repoType = typeof(IWriteRepository<>).MakeGenericType(domainModel.GetType());
//                    object repo = _container.GetInstance(repoType);

//                    ValidateConcurrency(root, state, repo);
//                    AdjustWhenSave(root, state);
//                    Save(root, state, repo);
//                    AddDomainEvents(root, state);
//                }
//            }
//        }

//        private ModelState GetAggregateRootState(IAggregateRoot root)
//        {
//            if (root.Deleted) return ModelState.Deleted;
//            if (root.Dirty && root.RecordVersion == 0) return ModelState.Added;
//            if (root.Dirty && root.RecordVersion > 0) return ModelState.Modified;

//            return ModelState.Unchanged;
//        }

//        private void ValidateConcurrency(IAggregateRoot root, ModelState state, object repo)
//        {
//            var method = repo.GetType().GetMethod("GetStored", new Type[] { root.GetType() });

//            IAggregateRoot storedRoot = method.Invoke(repo, new object[] { root }) as IAggregateRoot;

//            bool valid = true;

//            if (state == ModelState.Added && storedRoot != null) valid = false;
//            if (state == ModelState.Modified && root.RecordVersion != storedRoot.RecordVersion) valid = false;
//            if (state == ModelState.Deleted && root.RecordVersion != storedRoot.RecordVersion) valid = false;            

//            if (!valid) throw new Exception("concurrency exception.");
//        }

//        private void AdjustWhenSave(IAggregateRoot root, ModelState state)
//        {
//            if (state == ModelState.Unchanged) return;

//            VirtualObject<IAggregateRoot> vRoot = new VirtualObject<IAggregateRoot>(root);

//            vRoot
//                .Set(m => m.RecordVersion, (m, p) => m.RecordVersion + 1)
//                .Set(m => m.Dirty, (m, p) => false);
//        }

//        private void Save(IAggregateRoot root, ModelState state, object repo)
//        {
//            if (state == ModelState.Deleted)
//            {
//                var method = repo.GetType().GetMethod("Delete", new Type[] { root.GetType() });
//                method.Invoke(repo, new object[] { root });
//            }
//            else if (state == ModelState.Added)
//            {
//                var method = repo.GetType().GetMethod("Insert", new Type[] { root.GetType() });
//                method.Invoke(repo, new object[] { root });
//            }
//            else if (state == ModelState.Modified)
//            {
//                var method = repo.GetType().GetMethod("Update", new Type[] { root.GetType() });
//                method.Invoke(repo, new object[] { root });
//            }
//            else
//            {
//                return;
//            }
//        }

//        private void AddDomainEvents(IAggregateRoot root, ModelState state)
//        {
//            if (state == ModelState.Unchanged) return;
//            else DomainEventList.Add(root);
//        }

//        protected override void Dispose(bool completed)
//        {
//            _connection.DisposeTran();
//        }
//    }
//}