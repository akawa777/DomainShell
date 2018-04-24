using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test
{
    public interface IConnection : IDisposable
    {
        IDbCommand CreateCommand();
    }

    public interface IAggregateRoot : IDomainEventAuthor<IDomainEvent>
    {
        ModelState State { get; }

        bool Deleted { get; }

        string LastUpdate { get; }
    }
}