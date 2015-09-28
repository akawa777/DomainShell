using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.Command;

namespace DomainDesigner.Tests.DomainShell
{
    public class AddPersonCommand : ICommand<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UpdatePersonCommand : ICommand<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PersonCommandHandler : ICommandHandler<AddPersonCommand>, ICommandHandler<UpdatePersonCommand>
    {
        public PersonCommandHandler(PersonReadRepository repository, UnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        private PersonReadRepository _repository;
        private UnitOfWork _unitOfWork;

        public void Handle(AddPersonCommand command)
        {
            Person person = new Person();

            person.Add();

            _unitOfWork.Save(person);

            _result.Set(command, true);
        }

        private CommandResult _result = new CommandResult();

        public CommandResult CommandResult
        {
            get { return _result; }
        }

        public void Handle(UpdatePersonCommand command)
        {
            Person person = _repository.Load(command.Id);

            person.Update();

            _unitOfWork.Save(person);

            _result.Set(command, true);
        }
    }
}
