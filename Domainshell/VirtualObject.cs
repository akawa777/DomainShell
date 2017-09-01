using System;

namespace Domainshell
{
    public class VirtualObject<TElement> where TElement : class
    {
        public VirtualObject()
        {
            _element = Activator.CreateInstance(typeof(TElement), true) as TElement;
        }

        public VirtualObject(TElement element)
        {
            _element = element;
        }

        private TElement _element;

        public TElement Element
        {
            get { return _element; }
        }

        public VirtualObject<TElement> Set<TProperty>(System.Linq.Expressions.Expression<Func<TElement, TProperty>> propertyLambda, Func<TElement, System.Reflection.PropertyInfo, object> getValue)
        {
            if (propertyLambda == null)
            {
                throw new ArgumentException(string.Format(
                    "propertyLambda is required.",
                    propertyLambda.ToString()));
            }
            if (getValue == null)
            {
                throw new ArgumentException(string.Format(
                    "getValue is required.",
                    propertyLambda.ToString()));
            }

            var memberExpression = propertyLambda.Body as System.Linq.Expressions.MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            if (memberExpression.Member is System.Reflection.PropertyInfo propertyInfo && propertyInfo.CanWrite)
            {      
                
                object value = getValue(_element, propertyInfo);

                if (TryChangeType<TProperty>(value, out object convertedValue))
                {
                    propertyInfo.SetValue(_element, convertedValue);
                }
                else
                {
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a field, can not change type of '{1}'.",
                        propertyLambda.ToString(),
                        typeof(TProperty).FullName));
                }
            }
            else
            {                
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property or can not write.",
                    propertyLambda.ToString()));
            }

            return new VirtualObject<TElement>(_element);
        }

        private bool TryChangeType<TConversion>(object value, out object convertedValue)
        {
            convertedValue = null;
            try
            {
                convertedValue = Convert.ChangeType(value, typeof(TConversion));

                return  true;                
            }
            catch
            {
                return false;
            }
        }
    }
}