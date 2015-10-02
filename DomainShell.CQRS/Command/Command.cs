using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;

namespace DomainShell.CQRS.Command
{
    public class CommandResult : MessageResult
    {
        public void Set<TCommand, TResult>(TCommand command, TResult result) where TCommand : ICommand<TResult>
        {
            (this as IMessageResult).Set(command, result);            
        }
    }

    public interface ICommand : IMessage
    {

    }

    public interface ICommand<TResult> : ICommand, IMessage<TResult>
    {

    }

    public interface ICommandHandler : IMessageHandler
    {
        CommandResult CommandResult { get; }
    }

    public interface ICommandHandler<TCommand> : ICommandHandler, IMessageHandler<TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
