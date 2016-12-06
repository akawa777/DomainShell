using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Infrastructure.Shared;

namespace DomainShell.Tests.Commerce.Infrastructure.Repositories
{
    public class PurchaseRepository : BaseWriteRepository<PurchaseEntity, int>, IPurchaseRepository
    {
        public PurchaseRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher)            
        {
            _session = session;
        }

        private ISession _session;

        public override PurchaseEntity Find(int id)
        {
            return null;
        }

        public override void Insert(PurchaseEntity aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public override void Update(PurchaseEntity aggregateRoot)
        {
            throw new NotImplementedException();
        }

        public override void Delete(PurchaseEntity aggregateRoot)
        {
            throw new NotImplementedException();
        }
    }
}
