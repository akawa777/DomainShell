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
    public class PersonReadRepository
    {
        public Person Load(string id)
        {
            Person person = null;

            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();

                command.CommandText = "select * from Person where Id = @id";

                DbParameter parameter = command.CreateParameter();

                parameter.ParameterName = "@id";
                parameter.Value = id;

                command.Parameters.Add(parameter);

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        person = new Person();

                        person.Id = id;
                        person.Name = reader["Name"].ToString();
                    }
                }
            }            

            return person;
        }

        public Person[] GetAll()
        {
            List<Person> persons = new List<Person>();

            using (DbConnection connection = DataStore.CreateConnection())
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                
                command.CommandText = "select * from Person order by Id";

                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Person person = new Person();

                        person.Id = reader["Id"].ToString();
                        person.Name = reader["Name"].ToString();

                        persons.Add(person);
                    }
                }
            }

            return persons.ToArray();
        }
    }
}
