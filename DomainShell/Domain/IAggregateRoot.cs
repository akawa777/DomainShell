using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IModel
    {

    }

    public interface IValue : IModel
    {
        string Value { get; }
    }

    public interface IEntity : IModel
    {

    }

    public interface IEntity<TIdentity> : IEntity
    {
        TIdentity Id { get; }
    }

    public interface IAggregateRoot : IEntity, IDomainEventCollection
    {

    }

    public interface IAggregateRoot<TIdentity> : IAggregateRoot, IEntity<TIdentity>
    {

    }
}