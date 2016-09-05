using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
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

        private DbCommand CreateDbCommand()
        {
            return _session.GetPort<DbConnection>().CreateCommand();
        }

        public PurchaseReadObject[] GetPurchases(string customerId)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            List<PurchaseReadObject> list = db.Query<PurchaseReadObject>(@"
                select 
                    *
                from 
                    Payment                
                where 
                    CustomerId = @CustomerId                
                order by 
                    PaymentId desc
            ", new { CustomerId = customerId }).List();

            return list.ToArray();
        }
    }
}
