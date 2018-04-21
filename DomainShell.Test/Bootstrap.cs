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
using DomainShell.Test.Domains.UserDomain;
using DomainShell.Test.Domains.OrderDomain;
using DomainShell.Test.Infras.UserInfra;
using DomainShell.Test.Infras.OrderInfra;
using System.Reflection;

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

            // start up for DomainShell >>
            container.Register<IDomainModelFactory, DomainModelFactoryFoundation>(Lifestyle.Scoped);
            container.Register<IDomainModelTracker, DomainModelTrackerFoundation>(Lifestyle.Scoped);       
            container.Register<ISession, SessionFoundation>(Lifestyle.Scoped);

            DomainModelFactory.Startup(container.GetInstance<IDomainModelFactory>);            
            DomainModelTracker.Startup(container.GetInstance<IDomainModelTracker>);                        
            Session.Startup(container.GetInstance<ISession>);
            // <<    

            container.Register<IDbConnection>(() => _databaseProvider.CreateConnection(), Lifestyle.Scoped);            
            container.Register<IConnection, SessionFoundation>(Lifestyle.Scoped);                        

            container.Register<IUserRepository, UserRepository>(Lifestyle.Scoped);
            container.Register<IOrderRepository, OrderRepository>(Lifestyle.Scoped);                         
            container.Register<IOrderReadRepository, OrderRepository>(Lifestyle.Scoped);                         

            container.Register<IOrderService, OrderService>(Lifestyle.Scoped);            

            container.Register<IDomainEventHandler<OrderRegisterdEvent>, OrderEventHandler>(Lifestyle.Scoped);            
            container.Register<IDomainEventHandler<OrderPaidExceptionEvent>, OrderEventHandler>(Lifestyle.Scoped);
            container.Register<IDomainEventHandler<OrderReadIssuedCertificateEvent>, OrderReadEventHandler>(Lifestyle.Scoped);

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
                    CertificateIssueCount integer
                );

                create table LoginUser (
                    UserId text primary key,                    
                    UserName text
                );

                insert into LoginUser values('user1', 'user1');
                insert into LoginUser values('user2', 'user2');
            ";

            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}