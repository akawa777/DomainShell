using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace DomainShell
{
    public abstract class VirtualProperty
    {
        public abstract string Name { get; }
        public abstract object Value { get; }
        public abstract PropertyInfo Property { get; }
    }

    public class VirtualObject<TMaterial> where TMaterial : class
    {
        private class InternalVirtualProperty : VirtualProperty
        {
            public InternalVirtualProperty(PropertyInfo property, Func<object> getValue)
            {
                _getValue = getValue;
                _property = property;
            }

            private Func<object> _getValue;
            private PropertyInfo _property;

            public override string Name { get { return Property.Name; } }
            public override object Value { get { return _getValue(); } }
            public override PropertyInfo Property { get { return _property; } }
        }

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

        public VirtualProperty GetProperty<TProperty>(Expression<Func<TMaterial, TProperty>> propertyLambda, Func<TMaterial, PropertyInfo, object> getValue = null)
        {            
            string propertyName = GetPropertyName(propertyLambda);
            PropertyInfo property = GetPropertyInfo(propertyName);

            if (getValue == null) getValue = (m, p) => property.GetValue(_material);            

            Func<object> get = () => getValue(Material, property);

            return new InternalVirtualProperty(property, get);
        }

        public VirtualObject<TProperty> Get<TProperty>(string propertyName) where TProperty : class
        {
            System.Reflection.PropertyInfo property = GetPropertyInfo(propertyName);

            TProperty propertyMaterial = property.GetValue(_material) as TProperty;

            return new VirtualObject<TProperty>(propertyMaterial);
        }

        public IEnumerable<VirtualObject<TProperty>> List<TProperty>(string propertyName) where TProperty : class
        {
            PropertyInfo property = GetPropertyInfo(propertyName);

            IEnumerable<TProperty> propertyMaterial = property.GetValue(_material) as IEnumerable<TProperty>;

            return propertyMaterial.Select(x => new VirtualObject<TProperty>(x));
        }

        public VirtualObject<TProperty> Get<TProperty>(Expression<Func<TMaterial, TProperty>> propertyLambda) where TProperty : class
        {
            string propertyName = GetPropertyName(propertyLambda);

            return Get<TProperty>(propertyName);
        }        

        public IEnumerable<VirtualObject<TProperty>> List<TProperty>(Expression<Func<TMaterial, IEnumerable<TProperty>>> propertyLambda) where TProperty : class
        {
            string propertyName = GetPropertyName(propertyLambda);

            return List<TProperty>(propertyName);
        }

        public VirtualObject<TMaterial> Set(string propertyName, Func<TMaterial, PropertyInfo, object> getValue)
        {
            if (getValue == null)
            {
                throw new ArgumentException(string.Format(
                    "getValue is required.",
                    propertyName.ToString()));
            }

            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);

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

        public VirtualObject<TMaterial> Set<TProperty>(Expression<Func<TMaterial, TProperty>> propertyLambda, Func<TMaterial, PropertyInfo, object> getValue)
        {
            string propertyName = GetPropertyName(propertyLambda);

            return Set(propertyName, getValue);
        }

        private PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentException(string.Format(
                    "propertyName is required.",
                    propertyName.ToString()));
            }

            PropertyInfo propertyInfo =
                _material.GetType()
                .GetProperty(
                    propertyName, System.Reflection.BindingFlags.Instance 
                    | System.Reflection.BindingFlags.GetProperty 
                    | System.Reflection.BindingFlags.Public 
                    | System.Reflection.BindingFlags.NonPublic);

            if (!(propertyInfo != null && propertyInfo.CanWrite))
            {  
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property or can not write.",
                    propertyName.ToString()));    
            }

            return propertyInfo;
        }

        private string GetPropertyName<TProperty>(Expression<Func<TMaterial, TProperty>> propertyLambda)
        {
            if (propertyLambda == null)
            {
                throw new ArgumentException(string.Format(
                    "propertyLambda is required.",
                    propertyLambda.ToString()));
            }

            var memberExpression = propertyLambda.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
            }

            return memberExpression.Member.Name;
        }

        private bool TryChangeType(Type conversionType, object value, out object convertedValue)
        {
            MethodInfo method = 
                this.GetType()
                .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .Where(x => x.IsGenericMethod && x.Name == "TryChangeType")
                .First();

            object[] parameters = new object[] { value, null };
            object rtn = method.MakeGenericMethod(conversionType).Invoke(this, parameters);

            convertedValue = parameters[1];

            return (bool)rtn;
        }

        private bool TryChangeType<TConversion>(object value, out object convertedValue)
        {
            convertedValue = null;
            try
            {
                if (value == null && typeof(TConversion).IsClass)
                {
                    convertedValue = null;
                    return true;
                }

                if (value != null && Is<TConversion>(value))
                {
                    convertedValue = value;
                    return true;
                }                
                
                convertedValue = Convert.ChangeType(value, typeof(TConversion));

                return  true;                
            }
            catch
            {
                return false;
            }
        } 

        private bool Is<TConversion>(object value)     
        {
            return value is TConversion;
        }
    }
}