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

    public interface IValidationSpec<TTarget, TError> : 
        ISpecification
    {
        bool Validate(TTarget target, out TError[] errors);
    }

    public interface ICreationSpec<TTarget, TConstructorParameters> : 
        ISpecification
    {
        TConstructorParameters ConstructorParameters();
        void Satisfied(TTarget target);        
    }

    public interface ICreationWithSpec<TTarget> : ISpecification
    {
        void Satisfied(TTarget target);        
    }
}
