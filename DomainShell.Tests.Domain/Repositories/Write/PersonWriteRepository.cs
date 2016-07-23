﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Infrastructure;
using DomainShell.Tests.Domain.Models;
using System.Data.Common;

namespace DomainShell.Tests.Domain.Repositories.Write
{ 
    public class PersonWriteRepository
    {
        private static object o = new object();

        public void Add(Person person)
        {
            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                using (DbTransaction tran = connection.BeginTransaction())
                {
                    DbCommand command = connection.CreateCommand();

                    command.CommandText = "insert into Person(Name) values (@name)";

                    DbParameter parameter = command.CreateParameter();

                    parameter.ParameterName = "@name";
                    parameter.Value = person.Name;

                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();

                    command.CommandText = "select max(id) from Person";

                    string id = command.ExecuteScalar().ToString();

                    person.Id = id;

                    tran.Commit();
                }
            }
        }

        public void Update(Person person)
        {
            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                Update(person, connection);
            }
        }

        public void Update(Person person, object connection)
        {
            DbCommand command = (connection as DbConnection).CreateCommand();

            command.CommandText = "update Person set Name = @name where Id = @id";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = person.Id;

            command.Parameters.Add(parameter);

            parameter = command.CreateParameter();

            parameter.ParameterName = "@name";
            parameter.Value = person.Name;

            command.Parameters.Add(parameter);

            command.ExecuteNonQuery();
        }

        public void Delete(Person person)
        {
            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();

                command.CommandText = "delete from Person where Id = @id";

                DbParameter parameter = command.CreateParameter();

                parameter.ParameterName = "@id";
                parameter.Value = person.Id;

                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }
        }
    }
}
