using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Payment;

namespace DomainShell.Tests.Infrastructure.Payment
{    
    public class PaymentRepository : IRepositroy<PaymentModel>
    {
        public PaymentRepository(Session session)
        {
            _session = session;
        }

        private Session _session;  

        public void Save(PaymentModel payment)
        {
            if (payment.State == State.Created)
            {
                Create(payment);
            }

            payment.Accepted();
        }

        private void Create(PaymentModel payment)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                insert into Payment (PaymentDate, CustomerId, CreditCardNo, CreditCardHolder, CreditCardExpirationDate, ShippingAddress, Postage) 
                values (@PaymentDate, @CustomerId, @CreditCardNo, @CreditCardHolder, @CreditCardExpirationDate, @ShippingAddress, @Postage) 
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@PaymentDate";
            param.Value = payment.PaymentDate;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = payment.CustomerId;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardNo";
            param.Value = payment.CreditCardNo;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardHolder";
            param.Value = payment.CreditCardHolder;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CreditCardExpirationDate";
            param.Value = payment.CreditCardExpirationDate;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@ShippingAddress";
            param.Value = payment.ShippingAddress;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@Postage";
            param.Value = payment.Postage;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();

            command.CommandText = @"
                select PaymentId from Payment where ROWID = last_insert_rowid();         
            ";

            var id = command.ExecuteScalar();

            payment.PaymentId = id.ToString();

            int paymentItemId = 1;
            foreach (PaymentItemModel item in payment.PaymentItemList)
            {
                command.CommandText = @"
                    insert into PaymentItem (PaymentId, PaymentItemId, ProductId, PriceAtTime, Number) values (@PaymentId, @PaymentItemId, @ProductId, @PriceAtTime, @Number) 
                ";

                param = command.CreateParameter();
                param.ParameterName = "@PaymentId";
                param.Value = payment.PaymentId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@PaymentItemId";
                param.Value = paymentItemId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ProductId";
                param.Value = item.ProductId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@PriceAtTime";
                param.Value = item.PriceAtTime;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Number";
                param.Value = item.Number;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();

                paymentItemId++;
            }
        }
    }
}
