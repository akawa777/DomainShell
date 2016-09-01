using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Customer;

namespace DomainShell.Tests.Infrastructure.Customer
{
    public class CustomerRepository
    {
        public CustomerRepository(Session session)
        {
            _session = session;            
        }

        private Session _session;

        public CustomerModel Find(string customerId)
        {   
            DagentDatabase db = new DagentDatabase(_session.GetConnection());

            CustomerRecord record = db.Query<CustomerRecord>("Customer", new { CustomerId = customerId }).Single();

            return record == null ? null : new CustomerModel(record);
        }
    }
}
