using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;


namespace DomainShell.Tests
{
    internal class DataStoreProvider
    {
        private static string _db = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\", "test.db");
        private static string _connectionString = "data source=" + _db;

        private static bool _inited = false;

        public static DbConnection CreateConnection()
        {
            if (!_inited)
            {
                Init();

                _inited = true;
            }

            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

            DbConnection connection = factory.CreateConnection();
            connection.ConnectionString = _connectionString;            

            return connection;
        }

        public static DbDataAdapter CreateDataAdapter(DbCommand selectCommand)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SQLite");

            DbDataAdapter adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = selectCommand;

            DbCommandBuilder builder = factory.CreateCommandBuilder();
            builder.DataAdapter = adapter;

            return adapter;
        }

        private static void Init()
        {
            if (File.Exists(_db))
            {
                File.Delete(_db);
            }

            File.Create(_db).Close();

            SQLiteConnection.CreateFile(_db);

            using (DbConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                CreateTable(connection);
            }
        }

        private static void CreateTable(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                        create table Person (
                            PersonId integer,
                            Name nvarchar(100),
                            EMail nvarchar(100),
                            ZipCode nvarchar(100),
                            City nvarchar(100),
                            primary key(PersonId)
                        );

                        create table History (
                            PersonId integer,
                            HistoryNo integer,
                            Content nvarchar(100),                            
                            primary key(PersonId, HistoryNo)
                        );

                        create table IdManege (
                            TableName nvarchar(100),
                            Id integer,
                            Guid nvarchar(100),
                            primary key (TableName, Id)
                        );
                    ";

                var ret = command.ExecuteNonQuery();
            }
        }
    }
}
