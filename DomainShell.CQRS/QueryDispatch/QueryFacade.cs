using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;
using DomainShell.CQRS.Query;

namespace DomainShell.CQRS.QueryDispatch
{   
    public interface IQueryFacade
    {
        TReturn Get<TReturn>(IQuery<TReturn> query);        
    }

    public class QueryFacade : IQueryFacade
    {
        protected Dictionary<Type, Func<object>> _handlerMap = new Dictionary<Type, Func<object>>();

        public void Register<TQuery, TReturn>(Func<IQueryHandler<TQuery, TReturn>> handler) where TQuery : IQuery<TReturn>
        {
            _handlerMap[typeof(TQuery)] = handler;
        }

        public void Register(Type queryType, Func<IQueryHandler> handler)
        {
            _handlerMap[queryType] = handler;
        }

        public TReturn Get<TReturn>(IQuery<TReturn> query)
        {
            dynamic handler = _handlerMap[query.GetType()]();
            return (TReturn)handler.Handle(query as dynamic);
        }
    }   
}
