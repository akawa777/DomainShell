using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain;

namespace DomainShell.Tests.Infrastructure.Daos
{
    public class SqlGenerator
    {
        public string Generate<TTarget>(PredicateNode<TTarget, Operator> predicate)
        {
            StringBuilder sql = new StringBuilder();

            if (predicate.IsAggregateNode)
            {
                foreach (var childPredicate in predicate.Predicates)
                {
                    if (sql.ToString() != string.Empty)
                    {
                        sql.Append(predicate.And ? " and " : " or ");
                        sql.Append(Environment.NewLine);
                    }

                    sql.Append(Generate<TTarget>(childPredicate));
                }

                if (!predicate.And)
                {
                    sql = new StringBuilder("(" + sql.ToString() + ")");
                }
            }
            else
            {
                if (predicate.Operator == Operator.Equal)
                {
                    sql.Append(string.Format("{0} = @{1}", predicate.PropertyName, predicate.ParameterName));
                }
                else if (predicate.Operator == Operator.NotEqual)
                {
                    sql.Append(string.Format("{0} != @{1}", predicate.PropertyName, predicate.ParameterName));
                }
                else if (predicate.Operator == Operator.Like)
                {
                    sql.Append(string.Format("{0} like @{1}", predicate.PropertyName, predicate.ParameterName));
                }
            }

            return sql.ToString();
        }
    }
}
