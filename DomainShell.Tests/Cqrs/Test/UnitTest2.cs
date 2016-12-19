using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Cqrs.Command;
using DomainShell.Tests.Cqrs.Query;
using DomainShell.Tests.Cqrs.Infrastructure;

namespace DomainShell.Tests.Cqrs.Test
{
    [TestClass]
    public class UnitTest2
    {
        public UnitTest2()
        {
            CommandResultReposiotry commandResultReposiotry = new CommandResultReposiotry();
            CustomerRepository customerRepository = new CustomerRepository();
            CustomerCommandHandler customerCommandHandler = new CustomerCommandHandler(customerRepository, commandResultReposiotry);
            CommandResultQueryaHandler commandResultQueryaHandler = new CommandResultQueryaHandler(commandResultReposiotry);

            CommandBus commandBus = new CommandBus();
            commandBus.Registerd<CustomerCreateCommand>(customerCommandHandler);

            QueryBus queryBus = new QueryBus();
            queryBus.Registerd<CommandResultQuery>(commandResultQueryaHandler);
            
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        private ICommandBus _commandBus = null;
        private IQueryBus _queryBus = null;

        [TestMethod]
        public void Test()
        {
            CustomerCreateCommand command = new CustomerCreateCommand();

            _commandBus.Send(command);

            CommandResultQuery<CustomerCreateCommandResult> query = new CommandResultQuery<CustomerCreateCommandResult>(command);

            CustomerCreateCommandResult result = _queryBus.Send(query);

            int id = result.CustomerId;
        }
    }
}
