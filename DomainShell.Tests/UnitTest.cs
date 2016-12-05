using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.App;

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

            app.Create(personCreationRequest);

            PersonUpdateRequest personUpdateRequest = new PersonUpdateRequest
            {
                Name = "xxx",
                City = "xxxx",
                Content = "xxxx"
            };

            app.Update(personUpdateRequest);

            PersonDeletionRequest personDeletionRequest = new PersonDeletionRequest
            {
                PersonId = "1"
            };

            app.Delete(personDeletionRequest);

            List<PersonViewResult> list = app.GetCollection().ToList();
        }

        [TestMethod]
        public void Test02()
        {
            PredicateNode<int> predicate1 = new PredicateNode<int>(1);
            PredicateNode<int> predicate2 = new PredicateNode<int>(2);

            PredicateNode<int> subPredicateNode1 = new OrPredicateNode<int>(predicate1, predicate2);

            PredicateNode<int> predicate3 = new PredicateNode<int>(3);
            PredicateNode<int> predicate4 = new PredicateNode<int>(4);

            PredicateNode<int> subPredicateNode2 = new OrPredicateNode<int>(predicate3, predicate4);

            PredicateNode<int> summaryPredicateNode1 = new AndPredicateNode<int>(subPredicateNode1, subPredicateNode2);

            PredicateNode<int> predicate5 = new PredicateNode<int>(5);
            PredicateNode<int> predicate6 = new PredicateNode<int>(6);

            PredicateNode<int> subPredicateNode3 = new OrPredicateNode<int>(predicate5, predicate6);

            PredicateNode<int> predicate7 = new PredicateNode<int>(7);
            PredicateNode<int> predicate8 = new PredicateNode<int>(8);

            PredicateNode<int> subPredicateNode4 = new OrPredicateNode<int>(predicate7, predicate8);

            PredicateNode<int> summaryPredicateNode2 = new AndPredicateNode<int>(subPredicateNode3, subPredicateNode4);

            PredicateNode<int> totalPredicateNode = new AndPredicateNode<int>(summaryPredicateNode1, summaryPredicateNode2);

            string expression;
            int no = 1;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            ParsePredicateNode(
                totalPredicateNode, 
                (p, n, d) => 
                {
                    d[string.Format("@name_{0}", n.ToString())] = p.Target;
                    return string.Format("name = @name_{0}", n.ToString());
                }  ,
                out expression, 
                parameters,
                ref no);
        }

        public void ParsePredicateNode<TTarget>(
             PredicateNode<TTarget> predicate,
             Func<PredicateNode<TTarget>, int, Dictionary<string, object>, string> get,
             out string expressoin,
             Dictionary<string, object> parameters,
             ref int parameterNo)
        {
            IPredicateNode<TTarget> predicateBody = predicate as IPredicateNode<TTarget>;

            expressoin = string.Empty;

            if (predicateBody.PredicateNodeList == null || predicateBody.PredicateNodeList.Count == 0)
            {
                expressoin = get(predicate, parameterNo, parameters);
                parameterNo++;
            }
            else
            {
                string mergeSubNode = string.Empty;

                foreach (PredicateNode<TTarget> subPredicateNode in predicateBody.PredicateNodeList)
                {
                    string subNode;

                    ParsePredicateNode<TTarget>(subPredicateNode, get, out subNode, parameters, ref parameterNo);

                    if (mergeSubNode != string.Empty)
                    {
                        mergeSubNode += predicateBody.And ? " and " : " or ";
                    }

                    mergeSubNode += subNode;
                }

                mergeSubNode = " ( " + mergeSubNode + " ) ";

                expressoin += mergeSubNode;
            }
        }
    }
}
