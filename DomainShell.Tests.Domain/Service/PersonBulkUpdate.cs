using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Extension;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Repositories.Read;
using DomainShell.Tests.Domain.Infrastructure;

namespace DomainShell.Tests.Domain.Service
{
    public class PersonBulkUpdate
    {
        private PersonReadRepository _repository = new PersonReadRepository();

        public class Result
        {
            public bool Success { get; set; }
            public List<PersonModel> ErrorPersons { get; set; }
        }

        public Result BulkUpdate(string[] ids, string name)
        {
            if (ids == null)
            {
                return new Result 
                {
                    Success = false,
                    ErrorPersons = new List<PersonModel>() 
                };
            }

            List<PersonModel> errors = new List<PersonModel>();     

            using (ITransaction tran = TransactionProvider.BeginTran<PersonModel>())
            {
                foreach (string id in ids)
                {
                    PersonModel person = _repository.Get(id, tran.Session());
                    person.Name = name;

                    if (!person.UpdateInTran(tran.Session()))
                    {
                        errors.Add(person);
                    }
                }

                if (errors.Count == 0)
                {
                    tran.Complete();
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