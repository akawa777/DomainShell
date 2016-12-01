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
            CartProxy proxy = aggregateRoot as CartProxy;

            if (proxy.Transient && proxy.Deleted)
            {
                return;
            }

            if (!proxy.OnceVerified)
            {
                throw new Exception("not verified");
            }

            if (proxy.Transient)
            {

            }
            else if (!proxy.Deleted)
            {

            }
            else if (proxy.Deleted)
            {

            }

            proxy.Transient = false;

            foreach (IDomainEvent domainEvent in proxy.GetEvents())
            {
                _domainEventDispatcher.Dispatch(domainEvent);
            }

            proxy.ClearEvents();

            proxy.OnceVerified = false;
        }

        public CartEntity Find(CartId id)
        {
            return null;
        }
    }
}
