using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain;

namespace DomainShell.Tests.Infrastructure.Services
{
    internal class PersonSql
    {
        public string Sql(PersonPredicate predicate, out Dictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            string sql = @"
                select
                    Person.PersonId,
                    Person.Name,
                    Person.City
                    History.HistoryNo,
                    History.Content
                from
                    Person
                left join
                    Hisotry
                on
                    Person.PersonId = History.HistoryNo
                {0}
                order by
                    PersonId,
                    History.HistoryNo
            ";

            StringBuilder where = new StringBuilder();

            int suffix = 1;

            foreach (KeyValuePair<PersonPredicateItem, object> keyValue in predicate)
            {
                if (where.ToString() != string.Empty)
                {
                    where.Append(predicate.And ? " and " : " or ");
                    where.Append(Environment.NewLine);
                }

                string paramName = string.Empty;
                if (keyValue.Key == PersonPredicateItem.LikeName)
                {
                    paramName = string.Format("@name_{0}", suffix);
                    where.Append(string.Format("Person.Name like {0}", paramName));
                }
                else if (keyValue.Key == PersonPredicateItem.City)
                {
                    paramName = string.Format("@city_{0}", suffix);
                    where.Append(string.Format("Person.City = {0}", paramName));
                }

                if (paramName != string.Empty)
                {
                    parameters[paramName] = keyValue.Value;
                }
            }

            if (where.ToString() != string.Empty)
            {
                sql = string.Format(sql, where);
            }

            return sql;
        }
    }
}
