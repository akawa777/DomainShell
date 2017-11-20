using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using System.Data;

namespace DomainShell.Test.Domains
{
    public interface IConnection : IDisposable
    {
        IDbCommand CreateCommand();
    }

    public interface IAggregateRoot
    {
        int RecordVersion { get; }

        Dirty Dirty { get; }

        bool Deleted { get; }
    }

    public enum ModelState
    {
        Added,
        Deleted,
        Modified,
        Unchanged
    }
    
    public interface IWriteRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
       void Save(TAggregateRoot aggregate);
    }
}