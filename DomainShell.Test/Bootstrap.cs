using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Microsoft.Data.Sqlite;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainShell.Test
{
    public static class Bootstrap
    {
        public static Container Container { get; private set; }
        private static string _databaseFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database.sqlite");

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

        static Bootstrap()
        {
            LaunchDatabase();
            LaunchContainer();
        }

        private static string GetConnectionString()
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();

            builder.DataSource = _databaseFile;
            builder.Mode = SqliteOpenMode.ReadWrite;
            
            return builder.ConnectionString;
        }

        private static void LaunchContainer()
        {
            Container container = new Container();        
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();             
            
            IDbConnection connection = new SqliteConnection(GetConnectionString());
            connection.Open();

            container.Register<IDbConnection>(() => connection, Lifestyle.Singleton);

            
            container.Register<IDomainEventPublisher, DomainEventFoundation>(Lifestyle.Scoped);
            container.Register<IDomainAsyncEventPublisher, DomainEventFoundation>(Lifestyle.Scoped);
            container.Register<IDomainExceptionEventPublisher, DomainEventFoundation>(Lifestyle.Scoped);

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

            DomainEventPublisher.Startup(container.GetInstance<IDomainEventPublisher>);
            DomainAsyncEventPublisher.Startup(container.GetInstance<IDomainAsyncEventPublisher>);
            DomainExceptionEventPublisher.Startup(container.GetInstance<IDomainExceptionEventPublisher>);
            Session.Startup(container.GetInstance<ISession>);            

            Container = container;
        }

        private static void LaunchDatabase()
        {   
            using(var context = new DatabaseContext())
            {
                context.Database.EnsureCreated();
            }            

            var connection = new SqliteConnection(GetConnectionString());       
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
}