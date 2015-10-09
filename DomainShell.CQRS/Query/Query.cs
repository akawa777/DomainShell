using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;

namespace DomainShell.CQRS.Query
{
    public interface IQuery<TReturn>
    {

    }

    public interface IQueryHandler
    {
        
    }

    public interface IQueryHandler<TQuery, TReturn> : IQueryHandler where TQuery : IQuery<TReturn>
    {
        TReturn Handle(TQuery query);
    }
}
