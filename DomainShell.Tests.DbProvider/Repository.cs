using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using System.Data;
using System.Data.Common;

namespace DomainShell.Tests.DbProvider
{ 
    public class PersonReadRepository
    {
        public PersonReadRepository(DbConnection connection)
        {
            _connection = connection;    
        }

        private DbConnection _connection;

        public Person Load(int id)
        {
            DbCommand command = _connection.CreateCommand();

            command.CommandText = "select * from persons where id = @id";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = id;

            command.Parameters.Add(parameter);

            command.Connection.Open();

            Person person = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    person = new Person
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Version = reader["Version"].ToString()
                    };
                }
            }

            command.Connection.Close();

            return person;
        }

        public int GetNewId()
        {
            DbCommand command = _connection.CreateCommand();

            command.CommandText = "select max(id) from persons";

            command.Connection.Open();

            object value = command.ExecuteScalar();

            command.Connection.Close();

            int result;

            if (value == null || !int.TryParse(value.ToString(), out result))
            {
                return 0;
            }
            else
            {
                return result;
            }            
        }
    }

    public class PersonWriteRepository
    {
        public PersonWriteRepository(DbConnection connection)
        {
            _connection = connection;    
        }

        private DbConnection _connection;

        public void Add(Person person)
        {
            DbCommand command = _connection.CreateCommand();

            command.CommandText = "insert into persons values(@id, @name, @version)";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = person.Id;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@name";
            parameter.Value = person.Name;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@version";
            parameter.Value = person.Version;

            command.Parameters.Add(parameter);

            bool alreadyOpened = command.Connection.State == ConnectionState.Open;

            if (!alreadyOpened)
            {
                command.Connection.Open();
            }

            command.ExecuteNonQuery();

            if (!alreadyOpened)
            {
                command.Connection.Close();
            }
        }

        public void Update(Person person)
        {
            DbCommand command = _connection.CreateCommand();

            command.CommandText = "update persons set id = @id, name = @name, version = @newVersion where id = @id and version = @version";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = person.Id;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@name";
            parameter.Value = person.Name;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@newVersion";
            parameter.Value = Guid.NewGuid().ToString();

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@version";
            parameter.Value = person.Version;

            command.Parameters.Add(parameter);

            bool alreadyOpened = command.Connection.State == ConnectionState.Open;

            if (!alreadyOpened)
            {
                command.Connection.Open();
            }

            int rtn = command.ExecuteNonQuery();

            if (rtn == 0)
            {
                throw new Exception("concurrency exception.");
            }

            if (!alreadyOpened)
            {
                command.Connection.Close();
            }
        }

        public void Delete(Person person)
        {
            DbCommand command = _connection.CreateCommand();

            command.CommandText = "delete from persons where id = @id and version = @version";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = person.Id;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@version";
            parameter.Value = person.Version;

            command.Parameters.Add(parameter);

            bool alreadyOpened = command.Connection.State == ConnectionState.Open;

            if (!alreadyOpened)
            {
                command.Connection.Open();
            }

            int rtn = command.ExecuteNonQuery();

            if (rtn == 0)
            {
                throw new Exception("concurrency exception.");
            }

            if (!alreadyOpened)
            {
                command.Connection.Close();
            }
        }
    }

    public class TransactionProcessor : ITransactionProcessor
    {
        public TransactionProcessor(DbConnection connection)
        {
            _connection = connection;    
        }

        private DbConnection _connection;

        public void Execute(Action saveAction)
        {
            _connection.Open();

            using (DbTransaction tran = _connection.BeginTransaction())
            {
                saveAction();
            }

            _connection.Close();
        }
    }
}
