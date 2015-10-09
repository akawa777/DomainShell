using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Infrastructure;
using DomainShell.EventDispatch;
using DomainShell.CQRS.CommandDispatch;
using DomainShell.CQRS.QueryDispatch;

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

            TransactionProcessor transaction = new TransactionProcessor();

            _unitOfWork = new UnitOfWork(publisher, transaction);

            _bus = new CommandBus();
            _bus.Register<AddPersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            _bus.Register<UpdatePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            _bus.Register<RemovePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));

            _facade = new QueryFacade();

            _facade.Register<PersonListQuery, List<PersonData>>(() => new PersonListQueryHandler());
        }

        private PersonValidator _validator;
        private PersonReadRepository _repository;
        private UnitOfWork _unitOfWork;
        private CommandBus _bus;
        private QueryFacade _facade;

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
            PersonListQuery query = new PersonListQuery();

            List<PersonData> persons = _facade.Get(query);

            AddPersonCommand addCommand = new AddPersonCommand();
            
            addCommand.Name = "add";

            int newId = 0;

            _bus.Callback(addCommand, result =>
            {
                newId = result.NewId;
                Assert.AreEqual(true, result.Success);
            });

            _bus.Send(addCommand);

            UpdatePersonCommand updateCommand = new UpdatePersonCommand();            

            updateCommand.Id = newId;
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
