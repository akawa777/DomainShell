using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        public PurchaseRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
        {
            _session = session;
            _domainEventDispatcher = domainEventDispatcher;
        }

        private ISession _session;
        private IDomainEventDispatcher _domainEventDispatcher;

        public void Save(PurchaseEntity aggregateRoot)
        {
            PurchaseProxy purchase = aggregateRoot as PurchaseProxy;            

            foreach (IDomainEvent domainEvent in aggregateRoot.GetEvents())
            {
                _domainEventDispatcher.Dispatch(domainEvent);
            }

            aggregateRoot.ClearEvents();
        }

        public PurchaseEntity Find(int id)
        {
            return null;
        }
    }
}
