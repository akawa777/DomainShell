using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Infrastructure
{
    public interface IProxyModel
    {
        
    }

    public interface ITransient
    {
        bool Transient { get; set; }
    }

    public interface IDeleted
    {
        bool Deleted { get; }
    }

    public interface IOnceVerified
    {
        bool OnceVerified { get; set; }
    }

    public interface IAggregateProxyModel : IProxyModel, ITransient, IDeleted, IOnceVerified
    {

    }

    public interface IMementable<TMemento>
    {   
        TMemento Memento { get; }
    }

    public interface IProxyModel<TMemento> : IMementable<TMemento>
    {        
        void RewriteProxy();
        void RewriteMemento();
    }

    public interface IAggregateProxyModel<TMemento> : IAggregateProxyModel, IMementable<TMemento>
    {
        
    }
}
