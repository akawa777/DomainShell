using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.EventDispatch;
using DomainShell.CommandDispatch;
using DomainDesigner.Tests.DomainShell;

namespace DomainDesigner.Tests
{    
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            _repository = new PersonReadRepository();
            PersonWriteRepository writeRepository = new PersonWriteRepository();

            DomainEventPublisher publisher = new DomainEventPublisher();
            publisher.Register<PersonAddedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonUpdatedEvent>(() => new PersonEventHandler(writeRepository));

            _unitOfWork = new UnitOfWork(publisher);

            _bus = new CommandBus();
            _bus.Register<AddPersonCommand>(() => new PersonCommandHandler(_repository, _unitOfWork));
            _bus.Register<UpdatePersonCommand>(() => new PersonCommandHandler(_repository, _unitOfWork));
        }

        private PersonReadRepository _repository;
        private UnitOfWork _unitOfWork;
        private CommandBus _bus;

        [TestMethod]
        public void Main()
        {
            Person person = new Person();

            person.Add();

            _unitOfWork.Save(person);

            person = _repository.Load(1);

            person.Update();

            _unitOfWork.Save(person);
        }

        [TestMethod]
        public void Cqrs()
        {
            AddPersonCommand command = new AddPersonCommand();

            _bus.Callback(command, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(command);

            UpdatePersonCommand updateCommand = new UpdatePersonCommand();

            _bus.Callback(updateCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(updateCommand);
        }

    }
}
