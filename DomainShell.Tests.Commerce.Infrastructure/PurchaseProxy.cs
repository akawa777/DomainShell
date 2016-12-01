using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class PurchaseProxy : PurchaseEntity, IProxyModel
    {
        public PurchaseProxy(int id) : base(id)
        {

        }
    }
}
