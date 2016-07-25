using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Infrastructure;
using DomainShell.Tests.Domain.Models;
using System.Data.Common;

namespace DomainShell.Tests.Domain.Repositories.Read
{ 
    internal class PersonReadRepository
    {
        public PersonModel Get(string id)
        {
            PersonModel person = null;

            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                person = Get(id, connection);
            }            

            return person;
        }

        public PersonModel Get(string id, object session)
        {
            PersonModel person = null;

            DbCommand command = (session as DbConnection).CreateCommand();

            command.CommandText = "select * from Person where Id = @id";

            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = "@id";
            parameter.Value = id;

            command.Parameters.Add(parameter);

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    person = new PersonModel();

                    person.Id = id;
                    person.Name = reader["Name"].ToString();
                }
            }

            return person;
        }

        public PersonModel[] GetAll()
        {
            List<PersonModel> persons = new List<PersonModel>();

            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                
                command.CommandText = "select * from Person order by Id";

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PersonModel person = new PersonModel();

                        person.Id = reader["Id"].ToString();
                        person.Name = reader["Name"].ToString();

                        persons.Add(person);
                    }
                }
            }

            return persons.ToArray();
        }

        public void LoadAll(DataTable table)
        {
            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();                

                command.CommandText = "select * from Person order by Id";

                DbDataAdapter adapter = DataStore.CreateDataAdapter(command);                

                adapter.Fill(table);
            }
        }
    }
}
