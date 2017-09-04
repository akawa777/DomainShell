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

            container.Register<IDomainModelMarker, DomainModelTrackerFoundation>(Lifestyle.Scoped);
            container.Register<IDomainModelTracker, DomainModelTrackerFoundation>(Lifestyle.Scoped);
            container.Register<IDomainEventList, DomainEventFoundation>(Lifestyle.Scoped);
            container.Register<IDomainEventPublisher, DomainEventFoundation>(Lifestyle.Scoped);

            container.Register<IDbConnection>(() => _databaseProvider.CreateConnection(), Lifestyle.Scoped);
            container.Register<IConnection, Connection>(Lifestyle.Scoped);
            container.Register<Connection>(Lifestyle.Scoped);

            container.Register<ISession, SessionFoundation>(Lifestyle.Scoped);

            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped);

            container.Register<IOrderValidator, OrderValidator>(Lifestyle.Scoped);
            container.Register<ICreditCardService, CreditCardService>(Lifestyle.Scoped);
            container.Register<IMailService, MailService>(Lifestyle.Scoped);

            container.Register<IDomainEventHandler<OrderCompletedEvent>, OrderCompletedEventHandler>(Lifestyle.Scoped);
            container.Register<IDomainEventHandler<OrderCompletedExceptionEvent>, OrderCompletedEventHandler>(Lifestyle.Scoped);

            container.Register<OrderCommandApp>(Lifestyle.Scoped);
            container.Register<OrderQueryApp>(Lifestyle.Scoped);

            container.Verify();

            DomainModelMarker.Startup(container.GetInstance<IDomainModelMarker>);
            DomainModelTracker.Startup(container.GetInstance<IDomainModelTracker>);
            DomainEventList.Startup(container.GetInstance<IDomainEventList>);
            DomainEventPublisher.Startup(container.GetInstance<IDomainEventPublisher>);
            Session.Startup(container.GetInstance<ISession>);

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
                    ProductName text,
                    Price numeric,
                    PayId text,
                    LastUserId text,
                    RecordVersion int
                )";

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

            command.CommandText = "drop table OrderForm";

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
                    ProductName nvarchar(100),
                    Price decimal,
                    PayId nvarchar(100),
                    LastUserId nvarchar(100),
                    RecordVersion int
                )";

            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}