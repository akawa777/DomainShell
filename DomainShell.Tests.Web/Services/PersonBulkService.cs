using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Repositories.Read;
using DomainShell.Tests.Web.Infrastructure;

namespace DomainShell.Tests.Web.Services
{
    public class PersonBulkService
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

            using (Tran tran = new Tran())
            {
                foreach (string id in ids)
                {
                    Person person = _repository.Load(id);
                    person.Name = name;

                    if (!person.Update())
                    {
                        errors.Add(person);
                    }
                }

                tran.Complete();
            }

            return new Result 
            {
                Success = true,
                ErrorPersons = errors 
            };
        }
    }
}