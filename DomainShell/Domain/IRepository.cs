﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IReadReposiory<TAggregateRoot, TIdentity>
    {
        TAggregateRoot Find(TIdentity id);
    }

    public interface IReadSpecReposiory<TTarget>
    {
        IEnumerable<TTarget> List(ISelectionSpec<TTarget> spec);
    }

    public interface IReadSpecReposiory<TTarget, TPredicate>
    {
        IEnumerable<TTarget> List(ISelectionSpec<TPredicate> spec); 
    }

    public interface IWriteRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {        
        void Save(TAggregateRoot aggregateRoot);
    }

    public interface ICollectionWriteRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {
        void Add(TAggregateRoot aggregateRoot);
        void Remove(TAggregateRoot aggregateRoot);
        void Save();
    }
}
