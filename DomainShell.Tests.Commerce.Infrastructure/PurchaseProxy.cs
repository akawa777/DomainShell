using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Infrastructure.Shared;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class PurchaseProxy : PurchaseEntity, IAggregateProxyModel, IVersion
    {
        public PurchaseProxy(int id)
            : base(id)
        {

        }

        public bool Transient { get; set; }

        public bool Deleted { get; set; }

        public bool OnceVerified { get; set; }

        public override void Validate(IValidationSpec<PurchaseEntity, string> spec)
        {
            base.Validate(spec);
            OnceVerified = true;
        }

        public int Version
        {
            get;
            set;
        }
    }
}
