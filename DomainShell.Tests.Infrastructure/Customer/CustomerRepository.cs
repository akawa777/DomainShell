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

            using (DbDataReader reader = command.ExecuteReader())
            {
                CustomerRecord record = new CustomerRecord();            

                while (reader.Read())
                {
                    record.CustomerId = reader["CustomerId"].ToString();
                    record.CustomerName = reader["CustomerName"].ToString();
                    record.Address = reader["Address"].ToString();
                    record.CreditCardNo = reader["CreditCardNo"].ToString();
                    record.CreditCardHolder = reader["CreditCardHolder"].ToString();
                    record.CreditCardExpirationDate = reader["CreditCardExpirationDate"].ToString();
                }

                return record == null ? null : new CustomerModel(record);                
            }
        }
    }
}
