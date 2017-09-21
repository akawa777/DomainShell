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
using DomainShell.Test.Infras;

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
            container.Register<IDomainEventPublisher, DomainEventFoundation>(Lifestyle.Scoped);            
            container.Register<ISession, SessionFoundation>(Lifestyle.Scoped); 

            DomainModelProxyFactory.Startup(container.GetInstance<IDomainModelProxyFactory>);            
            DomainModelTracker.Startup(container.GetInstance<IDomainModelTracker>);            
            DomainEventPublisher.Startup(container.GetInstance<IDomainEventPublisher>);
            Session.Startup(container.GetInstance<ISession>);   
            // <<
                        
            container.Register<IDbConnection>(() => _databaseProvider.CreateConnection(), Lifestyle.Scoped);            
            container.Register<IConnection, Connection>(Lifestyle.Scoped);            

            container.Register<OrderModel, OrderModelProxy>(Lifestyle.Transient);

            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped); 
            container.Register<IOrderCanceledRepository, OrderCanceledRepository>(Lifestyle.Scoped);
            container.Register<IOrderSummaryRepository, OrderSummaryRepository>(Lifestyle.Scoped);

            container.Register<IOrderBudgetCheckService, OrderBudgetCheckService>(Lifestyle.Scoped);
            container.Register<ICreditCardService, CreditCardService>(Lifestyle.Scoped);
            container.Register<IMailService, MailService>(Lifestyle.Scoped);

            container.Register<IDomainEventHandler<OrderCompletedEvent>, OrderEventHandler>(Lifestyle.Scoped);
            container.Register<IDomainEventHandler<OrderCompletedExceptionEvent>, OrderEventHandler>(Lifestyle.Scoped);
            container.Register<IDomainEventHandler<OrderCanceledEvent>, OrderEventHandler>(Lifestyle.Scoped);

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
                    ProductName text,
                    Price numeric,
                    CreditCardCode text,
                    PayId text,                    
                    RecordVersion int
                );

                create table OrderFormItem (
                    OrderId integer primary key,
                    OrderItemNo,                    
                    ProductId text,
                    Price numeric,
                    RecordVersion int
                );

                create table OrderFormCanceled (
                    OrderId int primary key,                    
                    UserId text,                    
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

                create table OrderBudget (
                    UserId text primary key,                    
                    BudgetAmount numeric
                );

                insert into LoginUser values('user1', 'user1', 1);
                insert into LoginUser values('user2', 'user2', 1);

                insert into OrderBudget values('user1', 99999);
                insert into OrderBudget values('user2', 99999);
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
                drop table OrderBudget;
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
                    ProductName nvarchar(100),
                    Price decimal,
                    CreditCardCode nvarchar(100),
                    PayId nvarchar(100),                    
                    RecordVersion int
                );

                create table OrderFormCanceled (
                    OrderId int primary key,
                    UserId nvarchar(100),
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

                create table OrderBudget (
                    UserId nvarchar(100) primary key,
                    BudgetAmount decimal
                );

                insert into LoginUser values('user1', 'user1', 1);
                insert into LoginUser values('user2', 'user2', 1);

                insert into OrderBudget values('user1', 99999);
                insert into OrderBudget values('user2', 99999);
            ";

            command.ExecuteNonQuery();            

            connection.Close();
        }
    }
}