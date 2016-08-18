using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;

namespace DomainShell.Tests.Infrastructure.Payment
{
    public class PaymentReadObject
    {
        public string PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }        
        public string ShippingAddress { get; set; }
        public decimal PaymentAmount { get; set;}        
    }

    public class PaymentReader
    {
        public PaymentReader(Session session)
        {
            _session = session;
        }

        private Session _session;

        public PaymentReadObject[] GetPayments(string customerId)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select 
                    Payment.PaymentId, 
                    Payment.PaymentDate, 
                    Payment.CustomerId, 
                    Payment.ShippingAddress, 
                    Payment.Postage + sum(PriceAtTime) + PaymentTax PaymentAmount
                from Payment
                left join PaymentItem on Payment.PaymentId = PaymentItem.PaymentId                
                where Payment.CustomerId = @CustomerId
                group by Payment.PaymentId, Payment.PaymentDate, Payment.CustomerId, Payment.ShippingAddress
                order by Payment.PaymentId desc
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;
            command.Parameters.Add(param);

            List<PaymentReadObject> list = new List<PaymentReadObject>();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PaymentReadObject item = new PaymentReadObject();

                    item.PaymentId = reader["PaymentId"].ToString();
                    item.PaymentDate = reader["PaymentDate"].ToString();
                    item.CustomerId = reader["CustomerId"].ToString();
                    item.ShippingAddress = reader["ShippingAddress"].ToString();
                    item.PaymentAmount = int.Parse(reader["PaymentAmount"].ToString());

                    list.Add(item);
                }

                return list.ToArray();
            }
        }
    }
}
