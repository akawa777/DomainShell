using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Customer;

namespace DomainShell.Tests.Infrastructure.Customer
{
    public class CustomerRepository : IRepositroy<CustomerModel>
    {
        public CustomerRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public CustomerModel Find(string customerId)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select * from Customer where CustomerId = @CustomerId
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;
            command.Parameters.Add(param);

            CustomerModel customer = new CustomerModel();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    customer.CustomerId = reader["CustomerId"].ToString();
                    customer.CustomerName = reader["CustomerName"].ToString();
                    customer.Address = reader["Address"].ToString();
                    customer.CreditCardNo = reader["CreditCardNo"].ToString();
                    customer.CreditCardHolder = reader["CreditCardHolder"].ToString();
                    customer.CreditCardExpirationDate = reader["CreditCardExpirationDate"].ToString();
                }

                return customer;
            }
        }

        public void Save(CustomerModel customer)
        {
            customer.State.UnChanged();
        }
    }
}
