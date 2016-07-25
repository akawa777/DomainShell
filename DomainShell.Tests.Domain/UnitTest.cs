using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Domain.Models;
using DomainShell.Tests.Domain.Service;

namespace DomainShell.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Main()
        {
            PersonModel person = new PersonModel();

            person.Name = "add";

            person.Add();

            PersonReader reader = new PersonReader();

            person = reader.Get(person.Id);

            Assert.AreEqual("add", person.Name);

            person.Name = "update";

            person.Update();

            person = reader.Get(person.Id);

            Assert.AreEqual("update", person.Name);

            person.Remove();

            person = reader.Get(person.Id);

            Assert.AreEqual(null, person);

            PersonModel[] persons = reader.GetAll();

            string[] ids = persons.Select(x => x.Id).ToArray();

            PersonBulkUpdate bulk = new PersonBulkUpdate();            

            bulk.BulkUpdate(ids, "bulk");

            persons = reader.GetAll();

            foreach (PersonModel item in persons)
            {
                Assert.AreEqual("bulk", item.Name);
            }
        }
    }
}
