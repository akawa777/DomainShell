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
    public class CartRepository : ICartRepository
    {
        public CartRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
        {
            _session = session;
            _domainEventDispatcher = domainEventDispatcher;
        }

        private ISession _session;
        private IDomainEventDispatcher _domainEventDispatcher;

        public void Save(CartEntity aggregateRoot)
        {
            CartProxy cart = aggregateRoot as CartProxy;
            cart.Transient = false;

            foreach (IDomainEvent domainEvent in aggregateRoot.GetEvents())
            {
                _domainEventDispatcher.Dispatch(domainEvent);
            }

            aggregateRoot.ClearEvents();
        }

        public CartEntity Find(CartId id)
        {
            return null;
        }
    }
}
