using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Shared
{
    public abstract class BaseRepository<TAggregateRoot, TIdentity> : IReadReposiory<TAggregateRoot, TIdentity>, IWriteRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot<TIdentity>
    {
        public BaseRepository(IDomainEventDispatcher domainEventDispatcher)
        {
            DomainEventDispatcher = domainEventDispatcher;
        }

        protected virtual IDomainEventDispatcher DomainEventDispatcher
        {
            get;
            set;
        }

        public abstract TAggregateRoot Find(TIdentity id);

        public virtual void Save(TAggregateRoot aggregateRoot)
        {
            IAggregateProxyModel proxy = aggregateRoot as IAggregateProxyModel;

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
                Insert(aggregateRoot);
            }
            else if (!proxy.Deleted)
            {
                TAggregateRoot stored = Find(aggregateRoot.Id);

                if (stored == null || (stored as IVersion).Version != (proxy as IVersion).Version)
                {
                    throw new Exception("concurrency error");
                }

                Update(aggregateRoot);
            }
            else if (proxy.Deleted)
            {
                TAggregateRoot stored = Find(aggregateRoot.Id);

                if (stored == null || (stored as IVersion).Version != (proxy as IVersion).Version)
                {
                    throw new Exception("concurrency error");
                }

                Delete(aggregateRoot);
            }

            proxy.Transient = false;

            foreach (IDomainEvent domainEvent in aggregateRoot.GetEvents())
            {
                DomainEventDispatcher.Dispatch(domainEvent);
            }

            aggregateRoot.ClearEvents();

            proxy.OnceVerified = false;
        }

        public abstract void Insert(TAggregateRoot aggregateRoot);

        public abstract void Update(TAggregateRoot aggregateRoot);

        public abstract void Delete(TAggregateRoot aggregateRoot);
    }
}
