using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.EventDispatch;
using DomainShell.CQRS.CommandDispatch;
using System.Data.Common;

namespace DomainShell.Tests.DbProvider
{    
    [TestClass]
    public class UnitTest
    {
        [TestInitialize]
        public void Init()
        {
            DbConnection connection = null;

            _validator = new PersonValidator();
            _repository = new PersonReadRepository(connection);
            PersonWriteRepository writeRepository = new PersonWriteRepository(connection);

            DomainEventPublisher publisher = new DomainEventPublisher();
            publisher.Register<PersonAddedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonUpdatedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonRemovedEvent>(() => new PersonEventHandler(writeRepository));

            TransactionProcessor transaction = new TransactionProcessor(connection);

            _unitOfWork = new UnitOfWork(publisher, transaction);

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
