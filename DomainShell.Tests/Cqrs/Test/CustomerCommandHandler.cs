using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DomainShell.Tests.Cqrs.Command;
using DomainShell.Tests.Cqrs.Infrastructure;

namespace DomainShell.Tests.Cqrs.Test
{
    public class CustomerCreateCommand : ICommand<CustomerCreateCommandResult>
    {
        
    }

    public class CustomerCommandHandler : ICommandHandler<CustomerCreateCommand>
    {
        public CustomerCommandHandler(CustomerRepository repository, ICommandResultReposiotry commandResultReposiotry)
        {
            _repository = repository;
            _commandResultReposiotry = commandResultReposiotry;
        }

        private CustomerRepository _repository;
        private ICommandResultReposiotry _commandResultReposiotry;

        public void Handle(CustomerCreateCommand command)
        {
            Customer customer = new Customer();

            _repository.Save(customer);

            CustomerCreateCommandResult result = new CustomerCreateCommandResult();
            result.Command = command;
            result.CustomerId = customer.Id;

            _commandResultReposiotry.Save(result);
        }
    }

    public class CustomerCreateCommandResult : ICommandResult
    {
        public ICommand Command { get; set; }
        public int CustomerId { get; set; }
    }

    public class Customer
    {
        public Customer()
        {
            Id = DateTime.Now.Millisecond;
        }

        protected Customer(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }

    public class CustomerRepository
    {
        public void Save(Customer customer)
        {

        }
    }
}
