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
using DomainShell.Test.Apps;
using DomainShell.Test.Domains;
using DomainShell.Test.Domains.User;
using DomainShell.Test.Domains.Order;
using DomainShell.Test.Infras.User;
using DomainShell.Test.Infras.Order;
using MediatR;
using System.Reflection;

namespace DomainShell.Test
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }

        public enum DatabaseType
        {
            Sqlite,
            SqlServer
        }

        private static DatabaseType _databaseType = DatabaseType.SqlServer;
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
            else _databaseProvider = new SqlServerDatabaseProvider();
        }

        private static void LaunchContainer()
        {
            Container container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            // start up for DomainShell >>
            container.Register<IDomainModelProxyFactory, DomainModelProxyFactoryFoundation>(Lifestyle.Scoped);
            container.Register<IDomainModelTracker, DomainModelTrackerFoundation>(Lifestyle.Scoped);       
            container.Register<ISession, SessionFoundation>(Lifestyle.Scoped); 

            DomainModelProxyFactory.Startup(container.GetInstance<IDomainModelProxyFactory>);            
            DomainModelTracker.Startup(container.GetInstance<IDomainModelTracker>);                        
            Session.Startup(container.GetInstance<ISession>);   
            // <<

            // MediatR >>                        
            container.RegisterSingleton<IMediator, Mediator>();
            container.RegisterCollection(typeof(INotificationHandler<>), Assembly.GetExecutingAssembly());
            container.RegisterCollection(typeof(IAsyncNotificationHandler<>), Assembly.GetExecutingAssembly());
            container.RegisterCollection(typeof(ICancellableAsyncNotificationHandler<>), Assembly.GetExecutingAssembly());
            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));
            // <<
                        
            container.Register<IDbConnection>(() => _databaseProvider.CreateConnection(), Lifestyle.Scoped);            
            container.Register<IConnection, SessionFoundation>(Lifestyle.Scoped);            

            container.Register<OrderModel, OrderModelProxy>(Lifestyle.Transient);

            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped); 
            container.Register<IOrderCanceledRepository, OrderCanceledRepository>(Lifestyle.Scoped);
            container.Register<IMonthlyOrderRepository, MonthlyOrderRepository>(Lifestyle.Scoped);

            container.Register<IOrderBudgetCheckService, OrderBudgetCheckService>(Lifestyle.Scoped);
            container.Register<ICreditCardService, CreditCardService>(Lifestyle.Scoped);
            container.Register<IMailService, MailService>(Lifestyle.Scoped);

            //container.Register<IDomainEventHandler<OrderCompletedEvent>, OrderEventHandler>(Lifestyle.Scoped);
            //container.Register<IDomainEventHandler<OrderCompletedExceptionEvent>, OrderEventHandler>(Lifestyle.Scoped);
            //container.Register<INotificationHandler<OrderCanceledEvent>, OrderEventHandler>(Lifestyle.Scoped);

            container.Register<OrderCommandApp>(Lifestyle.Scoped);
            container.Register<OrderQueryApp>(Lifestyle.Scoped);                      
            
            container.Verify();
            Container = container;
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
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();

            builder.DataSource = _databaseFile;
            builder.Mode = SqliteOpenMode.ReadWrite;

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
                    PayId text,                    
                    RecordVersion int
                );

                create table OrderFormCanceled (
                    OrderId int primary key,                    
                    UserId text,       
                    OrderDate text,
                    ProductName text,
                    Price numeric,
                    CreditCardCode text,
                    PayId text,                    
                    RecordVersion int
                );

                create table LoginUser (
                    UserId text primary key,                    
                    UserName text,
                    RecordVersion int
                );

                create table MonthlyOrderBudget (
                    UserId text primary key,                    
                    Budget numeric
                );

                insert into LoginUser values('user1', 'user1', 1);
                insert into LoginUser values('user2', 'user2', 1);

                insert into MonthlyOrderBudget values('user1', 99999);
                insert into MonthlyOrderBudget values('user2', 99999);
            ";

            command.ExecuteNonQuery();

            connection.Close();
        }
    }

    public class SqlServerDatabaseProvider : DatabaseProvider
    {
        public SqlServerDatabaseProvider()
        {
            LaunchDatabase();
        }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(GetConnectionString());
        }

        private string GetConnectionString()
        {
            return @"Data Source = akawawin8\sqlserver2016; Initial Catalog = cplan_demo; Integrated Security = True";
        }

        private void LaunchDatabase()
        {
            var connection = CreateConnection();

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = @"
                drop table OrderForm;
                drop table OrderFormCanceled;
                drop table LoginUser;
                drop table MonthlyOrderBudget;
            ";

            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {

            }

            command.CommandText = @"
                create table OrderForm (
                    OrderId int identity primary key,
                    UserId nvarchar(100),
                    OrderDate nvarchar(8),
                    ProductName nvarchar(100),
                    Price decimal,
                    CreditCardCode nvarchar(100),
                    PayId nvarchar(100),                    
                    RecordVersion int
                );

                create table OrderFormCanceled (
                    OrderId int primary key,
                    UserId nvarchar(100),
                    OrderDate nvarchar(8),
                    ProductName nvarchar(100),
                    Price decimal,
                    CreditCardCode nvarchar(100),
                    PayId nvarchar(100),                    
                    RecordVersion int
                );

                create table LoginUser(
                    UserId nvarchar(100) primary key,
                    UserName nvarchar(100),
                    RecordVersion int
                );

                create table MonthlyOrderBudget (
                    UserId nvarchar(100) primary key,
                    Budget decimal
                );

                insert into LoginUser values('user1', 'user1', 1);
                insert into LoginUser values('user2', 'user2', 1);

                insert into MonthlyOrderBudget values('user1', 99999);
                insert into MonthlyOrderBudget values('user2', 99999);
            ";

            command.ExecuteNonQuery();            

            connection.Close();
        }
    }
}