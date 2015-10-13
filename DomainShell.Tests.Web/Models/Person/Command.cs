﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.CQRS.Command;
using DomainShell.CQRS.Infrastructure;

namespace DomainShell.Tests.Web.Models.Person
{
    public class AddPersonCommand : ICommand<AddPersonCommandResult>
    {        
        public string Name { get; set; }
    }

    public class AddPersonCommandResult
    {
        public int NewId { get; set; }
        public bool Success { get; set; }
    }

    public class UpdatePersonCommand : ICommand<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RemovePersonCommand : ICommand<bool>
    {
        public int Id { get; set; }
    }


    public class PersonCommandHandler : ICommandHandler<AddPersonCommand>, ICommandHandler<UpdatePersonCommand>, ICommandHandler<RemovePersonCommand>
    {
        public PersonCommandHandler(UnitOfWork unitOfWork, PersonReadRepository repository, PersonValidator validator)
        {            
            _unitOfWork = unitOfWork;
            _repository = repository;
            _validator = validator;
        }

        private UnitOfWork _unitOfWork;
        private PersonReadRepository _repository;
        private PersonValidator _validator;
        private CommandResult _result = new CommandResult();

        public CommandResult CommandResult
        {
            get { return _result; }
        }

        public void Handle(AddPersonCommand command)
        {
            Person person = new Person();

            person.Id = _repository.GetNewId();
            person.Name = command.Name;

            if (_validator.Validate(person))
            {
                person.Add();

                _unitOfWork.AsyncSave(person);

                _result.Set(command, new AddPersonCommandResult { NewId = person.Id, Success = true });
            }
            else
            {
                _result.Set(command, new AddPersonCommandResult());
            }
        }

        public void Handle(UpdatePersonCommand command)
        {
            Person person = _repository.Load(command.Id);

            person.Id = command.Id;
            person.Name = command.Name;

            if (_validator.Validate(person))
            {
                person.Update();
                _unitOfWork.AsyncSave(person);

                _result.Set(command, true);
            }
            else
            {
                _result.Set(command, false);
            }
        }

        public void Handle(RemovePersonCommand command)
        {
            Person person = _repository.Load(command.Id);

            if (_validator.Validate(person))
            {
                person.Remove();
                _unitOfWork.AsyncSave(person);

                _result.Set(command, true);
            }
            else
            {
                _result.Set(command, false);
            }
        }
    }
}
