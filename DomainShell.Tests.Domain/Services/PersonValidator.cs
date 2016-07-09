using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Repositories.Read;

namespace DomainShell.Tests.Domain.Services
{
    public class PersonValidator
    {
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