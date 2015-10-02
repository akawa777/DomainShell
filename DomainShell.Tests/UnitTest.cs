using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.EventDispatch;
using DomainShell.CQRS.CommandDispatch;

namespace DomainShell.Tests
{    
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            _validator = new PersonValidator();
            _repository = new PersonReadRepository();
            PersonWriteRepository writeRepository = new PersonWriteRepository();

            DomainEventPublisher publisher = new DomainEventPublisher();
            publisher.Register<PersonAddedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonUpdatedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonRemovedEvent>(() => new PersonEventHandler(writeRepository));

            _unitOfWork = new UnitOfWork(publisher);

            _bus = new CommandBus();
            _bus.Register<AddPersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            _bus.Register<UpdatePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            _bus.Register<RemovePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
        }

        private PersonValidator _validator;
        private PersonReadRepository _repository;
        private UnitOfWork _unitOfWork;
        private CommandBus _bus;

        [TestMethod]
        public void Main()
        {
            Person person = new Person();

            person.Id = _repository.GetNewId();
            person.Name = "add";

            if (_validator.Validate(person))
            {
                person.Add();
                _unitOfWork.Save(person);
            }

            person = _repository.Load(person.Id);

            person.Name = "update";

            if (_validator.Validate(person))
            {
                person.Update();
                _unitOfWork.Save(person);
            }

            person = _repository.Load(person.Id);

            if (_validator.Validate(person))
            {
                person.Remove();
                _unitOfWork.Save(person);
            }
        }

        [TestMethod]
        public void Cqrs()
        {
            AddPersonCommand addCommand = new AddPersonCommand();

            addCommand.Id = _repository.GetNewId();
            addCommand.Name = "add";

            _bus.Callback(addCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(addCommand);

            UpdatePersonCommand updateCommand = new UpdatePersonCommand();

            updateCommand.Id = addCommand.Id;
            updateCommand.Name = "update";

            _bus.Callback(updateCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(updateCommand);

            RemovePersonCommand removeCommand = new RemovePersonCommand();

            removeCommand.Id = updateCommand.Id;

            _bus.Callback<bool>(removeCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(removeCommand);
        }

    }
}
