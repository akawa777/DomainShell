using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Repositories.Read;

namespace DomainShell.Tests.Web.Services
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