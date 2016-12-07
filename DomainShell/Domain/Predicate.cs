using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace DomainShell.Domain
{
    public abstract class PredicateNode
    {
        public class Parameter
        {
            public Parameter(string name, object value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public object Value { get; private set; }
        }

        public object Value { get; protected set; }
        public bool And { get; protected set; }
    }

    public class PredicateNode<T, O> : PredicateNode
    {
        public PredicateNode(Expression<Func<T, object>> property, O @operator, object value)
            : this(property, @operator, value, true)
        {

        }

        protected PredicateNode(Expression<Func<T, object>> property, O @operator, object value, bool and)
        {
            Property = property;
            Operator = @operator;
            Value = value;
            And = and;
        }

        protected PredicateNode(PredicateNode<T, O>[] predicates, bool and)
        {
            Predicates = predicates;

            int suffixNo = 0;
            SetSuffixNo(ref suffixNo);
            And = and;
        }

        public Expression<Func<T, object>> Property { get; protected set; }
        public O Operator { get; protected set; }
        
        public string ParameterName 
        {
            get
            {
                if (IsAggregateNode)
                {
                    return string.Empty;
                }

                string propertyName = GetPropertyName(Property);

                if (SuffixNo == 0)
                {
                    return propertyName;
                }

                return propertyName + "_" + SuffixNo.ToString();
            }
        }
        public int SuffixNo { get; protected set; }

        public bool IsAggregateNode 
        { 
            get
            {
                return Predicates != null;
            }
        }

        public string PropertyName
        {
            get
            {
                return GetPropertyName(Property);
            }
        }

        public Parameter[] Parameters
        {
            get
            {
                List<Parameter> parameters = new List<Parameter>();

                SetParameters(ref parameters);

                return parameters.ToArray();
            }
        }

        public PredicateNode<T, O>[] Predicates
        {
            get;
            protected set;
        }

        protected void SetSuffixNo(ref int suffixNo)
        {            
            if (IsAggregateNode)
            {
                foreach (var predicate in Predicates)
                {
                    predicate.SetSuffixNo(ref suffixNo);
                }
            }
            else
            {
                suffixNo++;
                SuffixNo = suffixNo;
            }
        }        

        protected void SetParameters(ref List<Parameter> parameters)
        {
            if (IsAggregateNode)
            {
                foreach (var predicate in Predicates)
                {
                    predicate.SetParameters(ref parameters);
                }                
            }
            else
            {
                parameters.Add(new Parameter(ParameterName, Value));
            }
            
        }

        private string GetPropertyName(Expression<Func<T, object>> property)
        {
            return (Property.Body as System.Linq.Expressions.MemberExpression).Member.Name;
        }

        public bool Match(Expression<Func<T, object>> property)
        {
            return GetPropertyName(property) == GetPropertyName(Property);
        }        
    }

    public class AndPredicateNode<T, O> : PredicateNode<T, O>
    {
        public AndPredicateNode(params PredicateNode<T, O>[] predicates)
            : base(predicates, true)
        {

        }
    }

    public class OrPredicateNode<T, O> : PredicateNode<T, O>
    {
        public OrPredicateNode(params PredicateNode<T, O>[] predicates)
            : base(predicates, false)
        {

        }
    }
}
