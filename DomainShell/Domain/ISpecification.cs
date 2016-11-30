using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface ISpecification
    {
        
    }

    public interface ISpecification<TTarget> : 
        ISpecification
    {
        bool IsSatisfied(TTarget target);        
    }

    public interface ISelectionSpec<TPredicate> :
        ISpecification
    {
        TPredicate Predicate();
    }

    public interface IValidationSpec<TTarget> : 
        ISpecification
    {        
        bool Validate(TTarget target, out string[] errors);
    }

    public interface ICreationSpec<TTarget, TOptions> : 
        ISpecification
    {
        TOptions Options();
        void Satisfied(TTarget target);        
    }

    public interface ICreationWithSpec<TTarget> : ISpecification
    {
        void Satisfied(TTarget target);        
    }
}
