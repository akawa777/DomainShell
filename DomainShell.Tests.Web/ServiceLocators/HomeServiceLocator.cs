using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleInjector;
using DomainShell.EventDispatch;
using DomainShell.Infrastructure;
using DomainShell.CQRS.CommandDispatch;
using DomainShell.CQRS.QueryDispatch;
using DomainShell.Tests.Web.BizLogic;

namespace DomainShell.Tests.Web.ServiceLocators
{
    public class HomeServiceLocator : IServiceLocator
    {
        public HomeServiceLocator()
        {

            PersonReadRepository readRepository = new PersonReadRepository();
            PersonWriteRepository writeRepository = new PersonWriteRepository();

            PersonValidator validator = new PersonValidator();

            DomainEventPublisher publisher = new DomainEventPublisher();
            publisher.Register<PersonAddedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonUpdatedEvent>(() => new PersonEventHandler(writeRepository));
            publisher.Register<PersonRemovedEvent>(() => new PersonEventHandler(writeRepository));

            UnitOfWork unitOfWork = new UnitOfWork(publisher);

            CommandBus bus = new CommandBus();
            bus.Register<AddPersonCommand>(() => new PersonCommandHandler(unitOfWork, readRepository, validator));
            bus.Register<UpdatePersonCommand>(() => new PersonCommandHandler(unitOfWork, readRepository, validator));
            bus.Register<RemovePersonCommand>(() => new PersonCommandHandler(unitOfWork, readRepository, validator));

            QueryFacade facade = new QueryFacade();

            PersonDataReadRepository dataReadRepository = new PersonDataReadRepository();

            facade.Register<PersonListQuery, PersonData[]>(() => new PersonQueryHandler(dataReadRepository));
            facade.Register<PersonQuery, PersonData>(() => new PersonQueryHandler(dataReadRepository));

            CommandBus = bus;
            QueryFacade = facade;
        }

        public IQueryFacade QueryFacade { get; private set; }
        public ICommandBus CommandBus { get; private set; }
    }
}