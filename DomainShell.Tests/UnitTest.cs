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
        [TestMethod]
        public void Main()
        {
            Person person = new Person();
            
            person.Add();            

            person.Update();            

            bool success = person.Remove();

            Assert.AreEqual(true, success);
        }
    }
}
