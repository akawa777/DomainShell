using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Cqrs.Command;
using DomainShell.Tests.Cqrs.Infrastructure;

namespace DomainShell.Tests.Cqrs.Query
{
    public class CommandResultQuery : IQuery
    {
        public CommandResultQuery(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; private set; }

        public static IQuery<TReturn> Get<TReturn>(ICommand<TReturn> command)
        {
            return new CommandResultQuery<TReturn>(command);
        }
    }

    public class CommandResultQuery<TReturn> : CommandResultQuery, IQuery<TReturn>, IQueryType
    {
        public CommandResultQuery(ICommand<TReturn> command)
            : base(command)
        {

        }

        public Type GetQueryType()
        {
            return typeof(CommandResultQuery);
        }
    }

    public class CommandResultQueryaHandler : IQueryHandler<CommandResultQuery>
    {
        public CommandResultQueryaHandler(ICommandResultReposiotry repository)
        {
            _reposiotry = repository;
        }

        private ICommandResultReposiotry _reposiotry;

        public object Handle(CommandResultQuery query)
        {
            ICommandResult result = _reposiotry.Find(query.Command);

            return result;
        }
    }

    
}
