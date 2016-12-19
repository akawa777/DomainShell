﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IFactory<TAggregateRoot, TConstructorParameters> where TAggregateRoot : IAggregateRoot
    {
        TAggregateRoot Create(ICreationSpec<TAggregateRoot, TConstructorParameters> spec);
    }

    public interface ICollectionFactory<TAggregateRoot, TConstructorParameters> where TAggregateRoot : IAggregateRoot
    {
        IEnumerable<TAggregateRoot> CreateMultiple(ICreationSpec<TAggregateRoot, TConstructorParameters> spec);
    }
}
