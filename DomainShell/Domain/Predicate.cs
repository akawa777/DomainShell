using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Domain
{
    public interface IPredicateNode<TTarget>
    {
        bool And { get; set;  }
        TTarget Target { get; }
        List<IPredicateNode<TTarget>> PredicateNodeList { get; set; }
    }

    public class PredicateNode<TTarget> : IPredicateNode<TTarget>
    {
        public PredicateNode(TTarget target)
        {
            Target = target;
        }

        bool IPredicateNode<TTarget>.And
        {
            get;
            set;
        }

        public TTarget Target
        {
            get;
            private set;
        }

        List<IPredicateNode<TTarget>> IPredicateNode<TTarget>.PredicateNodeList
        {
            get;
            set;
        }
    }

    public class AndPredicateNode<TTarget> : PredicateNode<TTarget>
    {
        public AndPredicateNode(params PredicateNode<TTarget>[] predicateList) 
            : base(default(TTarget))
        {
            IPredicateNode<TTarget> predicate = this as IPredicateNode<TTarget>;

            predicate.And = true;
            predicate.PredicateNodeList = new List<IPredicateNode<TTarget>>();

            predicate.PredicateNodeList.AddRange(predicateList);
        }
    }

    public class OrPredicateNode<TTarget> : PredicateNode<TTarget>
    {
        public OrPredicateNode(params PredicateNode<TTarget>[] predicateList)
            : base(default(TTarget))
        {
            IPredicateNode<TTarget> predicate = this as IPredicateNode<TTarget>;

            predicate.And = false;
            predicate.PredicateNodeList = new List<IPredicateNode<TTarget>>();

            predicate.PredicateNodeList.AddRange(predicateList);
        }
    }
}
