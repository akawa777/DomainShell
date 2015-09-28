using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;

namespace DomainDesigner.Tests.DomainShell
{   
    public class PersonReadRepository
    {
        public Person Load(int id)
        {
            return new Person();
        }
    }

    public class PersonWriteRepository
    {
        public void Add(Person person)
        {

        }

        public void Update(Person person)
        {

        }
    }
}
