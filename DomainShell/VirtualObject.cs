using System;
using System.Linq;

namespace DomainShell
{
    public class VirtualObject<TMaterial> where TMaterial : class
    {
        public VirtualObject()
        {
            _material = Activator.CreateInstance(typeof(TMaterial), true) as TMaterial;
        }

        public VirtualObject(TMaterial material)
        {
            _material = material;
        }

        private TMaterial _material;

        public TMaterial Material
        {
            get { return _material; }
        }

        public VirtualObject<TProperty> Get<TProperty>(string propertyName) where TProperty : class
        {
            System.Reflection.PropertyInfo property = GetPropertyInfo(propertyName);

            TProperty propertyMaterial = property.GetValue(_material) as TProperty;

            return new VirtualObject<TProperty>(propertyMaterial);
        }

        public System.Collections.Generic.IEnumerable<VirtualObject<TProperty>> List<TProperty>(string propertyName) where TProperty : class
        {
            System.Reflection.PropertyInfo property = GetPropertyInfo(propertyName);

            System.Collections.Generic.IEnumerable<TProperty> propertyMaterial = property.GetValue(_material) as System.Collections.Generic.IEnumerable<TProperty>;

            return propertyMaterial.Select(x => new VirtualObject<TProperty>(x));
        }

        public VirtualObject<TProperty> Get<TProperty>(System.Linq.Expressions.Expression<Func<TMaterial, TProperty>> propertyLambda) where TProperty : class
        {
            System.Reflection.PropertyInfo property = GetPropertyInfo(propertyLambda);

            TProperty propertyMaterial = property.GetValue(_material) as TProperty;

            return new VirtualObject<TProperty>(propertyMaterial);
        }

        public System.Collections.Generic.IEnumerable<VirtualObject<TProperty>> List<TProperty>(System.Linq.Expressions.Expression<Func<TMaterial, System.Collections.Generic.IEnumerable<TProperty>>> propertyLambda) where TProperty : class
        {
            System.Reflection.PropertyInfo property = GetPropertyInfo(propertyLambda);

            System.Collections.Generic.IEnumerable<TProperty> propertyMaterial = property.GetValue(_material) as System.Collections.Generic.IEnumerable<TProperty>;

            return propertyMaterial.Select(x => new VirtualObject<TProperty>(x));
        }

        public VirtualObject<TMaterial> Set(string propertyName, Func<TMaterial, System.Reflection.PropertyInfo, object> getValue)
        {
            if (getValue == null)
            {
                throw new ArgumentException(string.Format(
                    "getValue is required.",
                    propertyName.ToString()));
            }

            System.Reflection.PropertyInfo propertyInfo = GetPropertyInfo(propertyName);

            object value = getValue(_material, propertyInfo);

            if (TryChangeType(propertyInfo.PropertyType, value, out object convertedValue))
            {
                propertyInfo.SetValue(_material, convertedValue);
            }
            else
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, can not change type of '{1}'.",
                    propertyName.ToString(),
                    propertyInfo.PropertyType.FullName));
            }

            return new VirtualObject<TMaterial>(_material);
        }

        public VirtualObject<TMaterial> Set<TProperty>(System.Linq.Expressions.Expression<Func<TMaterial, TProperty>> propertyLambda, Func<TMaterial, System.Reflection.PropertyInfo, object> getValue)
        {
            if (getValue == null)
            {
                throw new ArgumentException(string.Format(
                    "getValue is required.",
                    propertyLambda.ToString()));
            }

            System.Reflection.PropertyInfo propertyInfo = GetPropertyInfo(propertyLambda);

            object value = getValue(_material, propertyInfo);

            if (TryChangeType<TProperty>(value, out object convertedValue))
            {
                propertyInfo.SetValue(_material, convertedValue);
            }
            else
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, can not change type of '{1}'.",
                    propertyLambda.ToString(),
                    typeof(TProperty).FullName));
            }

            return new VirtualObject<TMaterial>(_material);
        }

        private System.Reflection.PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentException(string.Format(
                    "propertyLambda is required.",
                    propertyName.ToString()));
            }

            System.Reflection.PropertyInfo propertyInfo = typeof(TMaterial).GetProperty(propertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic);

            if (!(propertyInfo != null && propertyInfo.CanWrite))
            {  
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property or can not write.",
                    propertyName.ToString()));    
            }

            return propertyInfo;
        }

        private System.Reflection.PropertyInfo GetPropertyInfo<TProperty>(System.Linq.Expressions.Expression<Func<TMaterial, TProperty>> propertyLambda)
        {
            if (propertyLambda == null)
            {
                throw new ArgumentException(string.Format(
                    "propertyLambda is required.",
                    propertyLambda.ToString()));
            }

            var memberExpression = propertyLambda.Body as System.Linq.Expressions.MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            if (!(memberExpression.Member is System.Reflection.PropertyInfo propertyInfo && propertyInfo.CanWrite))
            {  
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property or can not write.",
                    propertyLambda.ToString()));    
            }

            return (memberExpression.Member as System.Reflection.PropertyInfo);
        }

        private bool TryChangeType(Type conversionType, object value, out object convertedValue)
        {
            convertedValue = null;
            try
            {
                convertedValue = Convert.ChangeType(value, conversionType);

                return  true;                
            }
            catch
            {
                return false;
            }
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