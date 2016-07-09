using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.IO;
using System.Data.Common;
using System.Data.SQLite;

namespace DomainShell.Tests.Domain.Infrastructure
{
    public class DataStore
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

        private static void Init()
        {            
            if (File.Exists(_db))
            {
                File.Delete(_db);                
            }

            File.Create(_db).Close();            

            SQLiteConnection.CreateFile(_db);

            using (DbConnection conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();  

                using (var command = conn.CreateCommand())  
                {  
                    command.CommandText = @"
                        create table Person(
                            Id integer primary key,
                            Name nvarchar(100)
                        )
                    ";

                    var ret = command.ExecuteNonQuery();  
                }

                using (DbCommand command = conn.CreateCommand())
                {
                    command.CommandText = "insert into Person(Name) values (@name)";                                        

                    for (int i = 0; i < 10; i++)
                    {
                        DbParameter parameter = command.CreateParameter();

                        parameter.ParameterName = "@name";
                        parameter.Value = (i + 1).ToString() + "_name";

                        command.Parameters.Add(parameter);
                        
                        var ret = command.ExecuteNonQuery();

                        command.Parameters.Clear();
                    }
                }  
            }
        }
    }
}
