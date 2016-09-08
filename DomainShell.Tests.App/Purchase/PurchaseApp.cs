using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;
using DomainShell.Tests.Domain.Purchase;
using DomainShell.Infrastructure;
using DomainShell.Tests.Infrastructure;
using DomainShell.Tests.Infrastructure.Common;
using DomainShell.Tests.Infrastructure.Cart;
using DomainShell.Tests.Infrastructure.Customer;
using DomainShell.Tests.Infrastructure.Product;
using DomainShell.Tests.Infrastructure.Purchase;

namespace DomainShell.Tests.App.Purchase
{
    public class PurchaseApp
    {
        public PurchaseApp()
        {
            _session = new Session(new SqliteSessionKernel());
            
            _purchasetReader = new PurchasetReader(_session);                        
        }

        private Session _session;        
        private PurchasetReader _purchasetReader;                

        public Purchase[] GetPurchases(PurchasesQuery query)
        {
            using (_session.Connect())
            {
                PurchaseReadObject[] readObjects = _purchasetReader.GetPurchases(query.CustomerId);

                return readObjects.Select(x => new Purchase
                {
                    PurchaseId = x.PurchaseId,
                    CustomerId = x.CustomerId,
                    PaymentDate = x.PaymentDate,
                    ShippingAddress = x.ShippingAddress,
                    PaymentAmount = x.PaymentAmount
                }).ToArray();
            }
        }
    }
}
