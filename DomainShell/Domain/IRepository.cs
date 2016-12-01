using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IIdReposiory<TAggregateRoot, TIdentity>
    {
        TAggregateRoot Find(TIdentity id);
    }

    public interface IReadReposiory<TTarget>
    {
        IEnumerable<TTarget> List(ISelectionSpec<TTarget> spec);
    }

    public interface IReadReposiory<TTarget, TPredicate>
    {
        IEnumerable<TTarget> List(ISelectionSpec<TPredicate> spec); 
    }

    public interface IRepository<TAggregateRoot, TIdentity> : IIdReposiory<TAggregateRoot, TIdentity>
        where TAggregateRoot : IAggregateRoot<TIdentity>        
    {        
        void Save(TAggregateRoot aggregateRoot);
    }

    public interface ICollectionRepository<TAggregateRoot, TIdentity> : IIdReposiory<TAggregateRoot, TIdentity>
        where TAggregateRoot : IAggregateRoot<TIdentity>
    {
        void Add(TAggregateRoot aggregateRoot);
        void Save();
    }
}
