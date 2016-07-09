using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Repositories.Read;

namespace DomainShell.Tests.Domain.Service
{
    public class PersonReader
    {
        private PersonReadRepository _repository = new PersonReadRepository();

        public Person Get(string id)
        {
            return _repository.Load(id);
        }

        public Person[] GetAll()
        {
            return _repository.GetAll();
        }
    }
}