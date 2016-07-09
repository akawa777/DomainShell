using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Repositories.Read;
using System.Data.SQLite;
using DomainShell.Tests.Web.Infrastructure;

namespace DomainShell.Tests.Web.Services
{
    public class PersonBulkUpdate
    {
        private PersonReadRepository _repository = new PersonReadRepository();

        public class Result
        {
            public bool Success { get; set; }
            public List<Person> ErrorPersons { get; set; }
        }

        public Result BulkUpdate(string[] ids, string name)
        {
            if (ids == null)
            {
                return new Result 
                {
                    Success = false,
                    ErrorPersons = new List<Person>() 
                };
            }

            List<Person> errors = new List<Person>();            

            using (SQLiteConnection connection = new SQLiteConnection(DataStore.ConnectionString))
            {
                connection.Open();

                using (SQLiteTransaction tran = connection.BeginTransaction())
                {
                    foreach (string id in ids)
                    {
                        Person person = _repository.Load(id);
                        person.Name = name;

                        if (!person.Update(connection))
                        {
                            errors.Add(person);
                        }
                    }

                    if (errors.Count == 0)
                    {
                        tran.Commit();
                    }
                }
            }

            return new Result 
            {
                Success = errors.Count == 0,
                ErrorPersons = errors 
            };
        }
    }
}