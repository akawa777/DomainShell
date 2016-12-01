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

    public interface IDeletable
    {
        bool Deleted { get; }
    }

    public interface IVerifiable
    {
        bool OnceVerified { get; set; }
    }

    public interface IAggregateProxyModel : IProxyModel, ITransient, IDeletable, IVerifiable
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
