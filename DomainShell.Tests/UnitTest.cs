using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.App;
using DomainShell.Tests.Domain;
using DomainShell.Tests.Infrastructure.Daos;

namespace DomainShell.Tests
{
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            SqliteSessionKernel.Config(DataStoreProvider.CreateConnection);
            _session = new DomainShell.Infrastructure.Session(new SqliteSessionKernel());
        }

        private ISession _session;

        [TestMethod]
        public void Test01()
        {
            PersonApp app = new PersonApp(_session);

            PersonCreationRequest personCreationRequest = new PersonCreationRequest
            {                
                Name = "xxx",
                ZipCode = "xxx",
                EMail = "xxx",
                Content = "xxx"
               
            };

            app.Execute(personCreationRequest);
            personCreationRequest.ZipCode = "xxxxxx";
            app.Execute(personCreationRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                Name = "xxx",
                City = "xxxx",
                Content = "xxxx"
            };

            app.Execute(personUpdateRequest);

            PersonViewRequest personViewRequest = new PersonViewRequest();

            var list = app.Execute(personViewRequest);

            var result = list.FirstOrDefault();

            PersonDeletionRequest personDeletionRequest = new PersonDeletionRequest
            {
                PersonId = "1"
            };

            app.Execute(personDeletionRequest);
        }

        [TestMethod]
        public void Test02()
        {
            string _name = "xxx";

            var p1 = new PredicateNode<PersonEntity, Operator>(x => x.Name, Operator.Equal, _name);

            var p2 = new PredicateNode<PersonEntity, Operator>(x => x.Name, Operator.NotEqual, _name);

            var pOr = new OrPredicateNode<PersonEntity, Operator>(p1, p2);

            var p3 = new PredicateNode<PersonEntity, Operator>(x => x.Name, Operator.Like, _name);

            var pAnd = new AndPredicateNode<PersonEntity, Operator>(pOr, p3);

            var parameters = pAnd.Parameters;

            SqlGenerator sqlGenerator = new SqlGenerator();

            var sql = sqlGenerator.Generate(pAnd);
        }
    }
}
