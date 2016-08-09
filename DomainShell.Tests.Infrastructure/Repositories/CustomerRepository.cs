using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Models;

namespace DomainShell.Tests.Infrastructure.Repositries
{
    public class CustomerRepository : IRepositroy<CustomerModel>
    {
        public CustomerRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public CustomerModel Get(string customerId)
        {
            return new CustomerModel();
        }

        public void Save(CustomerModel customer)
        {
            customer.Accepted();
        }
    }
}
