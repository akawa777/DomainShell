using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Cqrs.Command;

namespace DomainShell.Tests.Cqrs.Infrastructure
{
    public interface ICommandResultReposiotry
    {
        ICommandResult Find(ICommand command);
        void Save(ICommandResult commandResult);
    }

    public class CommandResultReposiotry : ICommandResultReposiotry
    {
        private Dictionary<ICommand, ICommandResult> _resultMap = new Dictionary<ICommand, ICommandResult>();

        public ICommandResult Find(ICommand command)
        {
            ICommandResult result = _resultMap[command];

            return result;
        }

        public void Save(ICommandResult commandResult)
        {
            _resultMap[commandResult.Command] = commandResult;
        }
    }
}
