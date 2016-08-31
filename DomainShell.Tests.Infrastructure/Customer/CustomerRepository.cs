using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
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
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select * from Customer where CustomerId = @CustomerId
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;
            command.Parameters.Add(param);

            CustomerEntity entity = new CustomerEntity();            

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    entity.CustomerId = reader["CustomerId"].ToString();
                    entity.CustomerName = reader["CustomerName"].ToString();
                    entity.Address = reader["Address"].ToString();
                    entity.CreditCardNo = reader["CreditCardNo"].ToString();
                    entity.CreditCardHolder = reader["CreditCardHolder"].ToString();
                    entity.CreditCardExpirationDate = reader["CreditCardExpirationDate"].ToString();
                }

                CustomerModel model = new CustomerModel();

                (model as IDomainModel<CustomerEntity>).Map(entity);

                return model;
            }
        }
    }
}
