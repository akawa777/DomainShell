using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Message;
using DomainShell.CQRS.Command;

namespace DomainShell.CQRS.CommandDispatch
{   
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
        void Callback<TResult>(ICommand<TResult> command, Action<TResult> action);
    }

    public class CommandBus : ICommandBus
    {
        private MessagePublisher _messagePublisher = new MessagePublisher();

        public void Register<TCommand>(Func<ICommandHandler<TCommand>> handler) where TCommand : ICommand
        {
            _messagePublisher.Register(handler);
        }

        public void Callback<TResult>(ICommand<TResult> command, Action<TResult> action)
        {
            _messagePublisher.Callback(command, action);
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            _messagePublisher.Publish(command, handler => (handler as ICommandHandler).CommandResult);
        }

        public void SetBeginHandle(Action<ICommand, ICommandHandler> beginHandle)
        {
            _messagePublisher.SetBeginHandle((@event, handler) => beginHandle(@event as ICommand, handler as ICommandHandler));
        }

        public void SetEndHandle(Action<ICommand, ICommandHandler, Exception> endHandle)
        {
            _messagePublisher.SetEndHandle((@event, handler, exception) => endHandle(@event as ICommand, handler as ICommandHandler, exception));
        }
    }   
}
