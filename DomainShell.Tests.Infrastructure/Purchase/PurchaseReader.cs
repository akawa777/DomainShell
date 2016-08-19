using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;

namespace DomainShell.Tests.Infrastructure.Purchase
{
    public class PurchaseReadObject
    {
        public string PurchaseId { get; set; }
        public string PaymentDate { get; set; }
        public string CustomerId { get; set; }        
        public string ShippingAddress { get; set; }
        public decimal PaymentAmount { get; set;}        
    }

    public class PurchasetReader
    {
        public PurchasetReader(Session session)
        {
            _session = session;
        }

        private Session _session;

        public PurchaseReadObject[] GetPurchases(string customerId)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                select 
                    *
                from 
                    Payment                
                where 
                    CustomerId = @CustomerId                
                order by 
                    PaymentId desc
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;
            command.Parameters.Add(param);

            List<PurchaseReadObject> list = new List<PurchaseReadObject>();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    PurchaseReadObject item = new PurchaseReadObject();

                    item.PurchaseId = reader["PaymentId"].ToString();
                    item.PaymentDate = reader["PaymentDate"].ToString();
                    item.CustomerId = reader["CustomerId"].ToString();
                    item.ShippingAddress = reader["ShippingAddress"].ToString();
                    item.PaymentAmount = decimal.Parse(reader["PaymentAmount"].ToString());

                    list.Add(item);
                }

                return list.ToArray();
            }
        }
    }
}
