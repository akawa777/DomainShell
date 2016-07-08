using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainShell.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {            
            _repository = new PersonReadRepository();            
        }

        private PersonReadRepository _repository;
        

        [TestMethod]
        public void Main()
        {
            Person person = new Person();
            
            person.Name = "add";

            person.Add();

            person = _repository.Load(person.Id);

            person.Name = "update";

            person.Update();

            person = _repository.Load(person.Id);

            person.Remove();
        }
    }
}
