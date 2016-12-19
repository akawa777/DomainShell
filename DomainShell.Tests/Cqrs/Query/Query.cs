using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Cqrs.Command;

namespace DomainShell.Tests.Cqrs.Query
{
    public interface IQuery
    {

    }

    public interface IQuery<TReturn> : IQuery
    {

    }

    public interface IQueryType
    {
        Type GetQueryType();
    }

    public interface IQueryHandler<TQuery> where TQuery : IQuery
    {
        object Handle(TQuery query);
    }

    public interface IQueryHandler<TQuery, TReturn> where TQuery : IQuery<TReturn>
    {
        TReturn Handle(TQuery query);
    }

    public interface IQueryBus
    {
        TReturn Send<TReturn>(IQuery<TReturn> query);
    }

    public class QueryBus : IQueryBus
    {
        private Dictionary<Type, Func<IQuery, object>> _handlerMap = new Dictionary<Type, Func<IQuery, object>>();

        public void Registerd<TQuery>(IQueryHandler<TQuery> handler) where TQuery : IQuery
        {
            _handlerMap[typeof(TQuery)] = query => handler.Handle((TQuery)query);
        }

        public void Registerd<TQuery, TReturn>(IQueryHandler<TQuery, TReturn> handler) where TQuery : IQuery<TReturn>
        {
            _handlerMap[typeof(TQuery)] = query => handler.Handle((TQuery)query);
        }

        public TReturn Send<TReturn>(IQuery<TReturn> query)
        {
            Type queryType;

            if (query is IQueryType)
            {
                queryType = (query as IQueryType).GetQueryType();
            }
            else
            {
                queryType = query.GetType();
            }

            object returnValue = _handlerMap[queryType](query);

            return (TReturn)returnValue;
        }
    }
}
