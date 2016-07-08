using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Web.Models;
using DomainShell.Tests.Web.Repositories.Read;

namespace DomainShell.Tests.Web.Services
{
    public class PersonValidator
    {
        private PersonReadRepository _reposiory = new PersonReadRepository();

        public bool Validate(Person person)
        {
            if (string.IsNullOrEmpty(person.Name))
            {
                return false;
            }

            return true;
        }
    }
}