using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Purchase;

namespace DomainShell.Tests.Infrastructure.Purchase
{
    public class PurchaseRepository : IWriteRepository<PurchaseModel>
    {
        public PurchaseRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public void Save(PurchaseModel purchase)
        {
            if (purchase.State == State.Added)
            {
                Create(purchase);
                purchase.Stored();
            }
        }

        private void Create(PurchaseModel purchase)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            db.Command<PurchaseModel>("Purchase", "PurchaseId").Insert(purchase);            

            foreach (PurchaseDetailModel detail in purchase.PurchaseDetails)
            {                
                db.Command<PurchaseDetailModel>("PurchaseDetail", "PurchaseId", "PurchaseDetailId").Insert(detail);
            }
        }
    }
}
