using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface ISpecification<TTarget>
    {
        bool IsSatisfied(TTarget target);        
    }

    public interface ISelectionSpec<TTarget, TPredicate>
    {
        TPredicate Predicate();
    }

    public interface ISelectionPredicateSpec<TTarget, TOperator> :
        ISelectionSpec<TTarget, PredicateNode<TTarget, TOperator>>
    {
        
    }

    public interface IValidationSpec<TTarget, TError>
    {
        bool Validate(TTarget target, out TError[] errors);
    }

    public interface ICreationSpec<TTarget, TConstructorParameters>
    {
        TConstructorParameters ConstructorParameters();
        void Satisfied(TTarget target);        
    }

    public interface ICreationWithSpec<TTarget>
    {
        void Satisfied(TTarget target);        
    }
}
