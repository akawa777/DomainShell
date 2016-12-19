using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Cqrs.Command
{
    public interface ICommand
    {

    }

    public interface ICommandResult
    {
        ICommand Command { get; }
    }

    public interface ICommand<ICommandResult> : ICommand
    {

    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }

    public interface ICommandBus
    {
        void Send(ICommand command);
    }

    public class CommandBus : ICommandBus
    {
        private Dictionary<Type, Action<ICommand>> _handlerMap = new Dictionary<Type, Action<ICommand>>();

        public void Registerd<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
            _handlerMap[typeof(TCommand)] = command => handler.Handle((TCommand)command);
        }

        public void Send(ICommand command)
        {
            _handlerMap[command.GetType()](command);
        }
    }
}
