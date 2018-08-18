using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DomainShell.Test.App;
using DomainShell.Test.Domain.OrderAggregate;
using DomainShell.Test.Domain.UserAggregate;
using DomainShell.Test.Infra.UserAggregate;
using DomainShell.Test.Infra.OrderAggregate;
using System.Reflection;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }

        public enum DatabaseType
        {
            Sqlite
        }

        private static DatabaseType _databaseType = DatabaseType.Sqlite;
        private static DatabaseProvider _databaseProvider = null;

        public static void StartUp(DatabaseType databaseType)
        {
            _databaseType = databaseType;

            LaunchDatabase();
            LaunchContainer();
        }

        private static void LaunchDatabase()
        {
            if (_databaseType == DatabaseType.Sqlite) _databaseProvider = new SqliteDatabaseProvider();            
        }

        private static void LaunchContainer()
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            
            container.Register<ISessionKernel, SessionKernel>(Lifestyle.Scoped);
            container.Register<ISessionExceptionCatcherKernel, SessionExceptionCatcherKernel>(Lifestyle.Scoped);
            container.Register<IModelStateTrackerKernel, ModelStateTrackerKernel>(Lifestyle.Scoped);
            container.Register<IDomainEventCacheKernel<IDomainEvent>, DomainEventCache>(Lifestyle.Scoped);
            container.Register<IDomainEventPublisherKernel<IAggregateRoot>, DomainEventPublisherKernel>(Lifestyle.Scoped);            

            container.Register(() => _databaseProvider.CreateConnection(), Lifestyle.Scoped);            
            container.Register<IConnection, SessionKernel>(Lifestyle.Scoped);            

            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped);                         
            container.Register<IOrderReadRepository, OrderRepository>(Lifestyle.Scoped);                         

            container.Register<IOrderService, OrderService>(Lifestyle.Scoped);            

            container.Register<IDomainEventHandler<OrderPaidEvent>, OrderEventHandler>(Lifestyle.Scoped);            
            container.Register<IDomainEventExceptionHandler<OrderPaidEvent>, OrderEventHandler>(Lifestyle.Scoped);            
            container.Register<IDomainEventAsyncHandler<UserRegisterdEvent>, UserEventHandler>(Lifestyle.Scoped);

            container.Register<ILogKernel, LogKernel>(Lifestyle.Scoped);

            container.Register<OrderCommandApp>(Lifestyle.Scoped);
            container.Register<OrderQueryApp>(Lifestyle.Scoped);

            container.Verify();
            Container = container;

            Session.Startup(container.GetInstance<ISessionKernel>);
            SessionExceptionCatcher.Startup(container.GetInstance<ISessionExceptionCatcherKernel>);
            ModelStateTracker.Startup(container.GetInstance<IModelStateTrackerKernel>);
            DomainEventPublisher.Startup(container.GetInstance<IDomainEventPublisherKernel<IAggregateRoot>>);            
            Log.Startup(container.GetInstance<ILogKernel>);
        }
    }

    public abstract class DatabaseProvider
    {
        public abstract IDbConnection CreateConnection();
    }

    public class SqliteDatabaseProvider : DatabaseProvider
    {
        private class DatabaseContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {                
                if (System.IO.File.Exists(_databaseFile))
                {
                    System.IO.File.Delete(_databaseFile);
                }

                optionsBuilder.UseSqlite($"Filename={_databaseFile}");                
            }
        }

        public SqliteDatabaseProvider()
        {
            LaunchDatabase();
        }

        private static string _databaseFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.sqlite");

        public override IDbConnection CreateConnection()
        {
            return new SqliteConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder
            {
                DataSource = _databaseFile,
                Mode = SqliteOpenMode.ReadWrite
            };

            return builder.ConnectionString;
        }

        private void LaunchDatabase()
        {
            using (var context = new DatabaseContext())
            {
                context.Database.EnsureCreated();
            }

            var connection = CreateConnection();
            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = @"
                create table OrderForm (
                    OrderId integer primary key,
                    UserId text,
                    OrderDate text,
                    ProductName text,
                    Price numeric,
                    CreditCardCode text,
                    PaymentId text,     
                    CertificateIssueCount integer,
                    SpecialOrderFlg integer,
                    LastUpdate text
                );

                create table LoginUser (
                    UserId text primary key,                    
                    UserName text,
                    PaymentPoint integer,
                    LastUpdate text
                );

                insert into LoginUser values('user1', 'user1', 0, '201801010000');
                insert into LoginUser values('user2', 'user2', 0, '201801010000');
            ";

            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}