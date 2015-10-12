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

namespace DomainShell.Tests.Web
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

            CommandBus bus = new CommandBus();
            bus.Register<AddPersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            bus.Register<UpdatePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));
            bus.Register<RemovePersonCommand>(() => new PersonCommandHandler(_unitOfWork, _repository, _validator));

            _bus = bus;

            QueryFacade facade = new QueryFacade();

            PersonDataReadRepository _readRepository = new PersonDataReadRepository();

            facade.Register<PersonListQuery, PersonData[]>(() => new PersonQueryHandler(_readRepository));
            facade.Register<PersonQuery, PersonData>(() => new PersonQueryHandler(_readRepository));

            _facade = facade;
        }

        private PersonValidator _validator;
        private PersonReadRepository _repository;
        private UnitOfWork _unitOfWork;
        private ICommandBus _bus;
        private IQueryFacade _facade;

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
            PersonListQuery listQuery = new PersonListQuery();

            PersonData[] persons = _facade.Get(listQuery);

            Assert.AreEqual(true, persons.Length > 0);

            AddPersonCommand addCommand = new AddPersonCommand();
            
            addCommand.Name = "add";

            _bus.Callback(addCommand, result =>
            {                
                Assert.AreEqual(true, result.Success);
                Assert.AreEqual(persons.Max(x => x.Id + 1), result.NewId);
            });

            _bus.Send(addCommand);

            PersonQuery query = new PersonQuery();
            query.Id = persons[0].Id;

            PersonData person = _facade.Get(query);

            UpdatePersonCommand updateCommand = new UpdatePersonCommand();

            updateCommand.Id = person.Id;
            updateCommand.Name = "update";

            _bus.Callback(updateCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(updateCommand);

            query = new PersonQuery();
            query.Id = persons[1].Id;

            person = _facade.Get(query);

            RemovePersonCommand removeCommand = new RemovePersonCommand();

            removeCommand.Id = person.Id;

            _bus.Callback<bool>(removeCommand, success =>
            {
                Assert.AreEqual(true, success);
            });

            _bus.Send(removeCommand);
        }

    }
}
