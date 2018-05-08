using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace DomainShell
{
    public interface IProxyObject<TMaterial>
    {   
        TMaterial Material { get; }
        PropertyInfo Property { get; }
        IProxyObject<TProperty> Get<TProperty>(Expression<Func<TMaterial, TProperty>> expression);
        IEnumerable<IProxyObject<TProperty>> List<TProperty>(Expression<Func<TMaterial, IEnumerable<TProperty>>> expression);
        IProxyObject<TMaterial> Set<TProperty>(Expression<Func<TMaterial, TProperty>> expression, Func<TMaterial, PropertyInfo, object> value);
    }

    public class ProxyObject<TMaterial> : IProxyObject<TMaterial>
    {
        public ProxyObject()
        {
            var material = Activator.CreateInstance(typeof(TMaterial), true);

            Material = (TMaterial)material;
        }
        public ProxyObject(TMaterial material)
        {
            Material = material;
        }

        private ProxyObject(TMaterial material, PropertyInfo property)
        {
            Material = material;
            Property = property;
        }

        public TMaterial Material { get; }

        public PropertyInfo Property { get; }

        public IProxyObject<TProperty> Get<TProperty>(Expression<Func<TMaterial, TProperty>> expression)
        {
            var property = GetProperty(typeof(TMaterial), expression);
            var material = (TProperty)property.Get(Material);
            return new ProxyObject<TProperty>(material, property);
        }

        public IEnumerable<IProxyObject<TProperty>> List<TProperty>(Expression<Func<TMaterial, IEnumerable<TProperty>>> expression)
        {
            var property = GetProperty(typeof(TMaterial), expression);
            var materials = (IEnumerable<TProperty>)property.Get(Material);
            return materials.Select(x => new ProxyObject<TProperty>(x, null));
        }

        public IProxyObject<TMaterial> Set<TProperty>(Expression<Func<TMaterial, TProperty>> expression, Func<TMaterial, PropertyInfo, object> value)
        {
            var property = GetProperty(typeof(TMaterial), expression);
            var valueObj = value(Material, property);
            property.Set(Material, valueObj);
            return this;
        }

        private PropertyInfo GetProperty<TProperty>(Type type, Expression<Func<TMaterial, TProperty>> expression)
        {
            var property = type.GetProperty((expression.Body as MemberExpression).Member.Name);            
            
            if (property == null && type.BaseType == typeof(object))
            {
                throw new ArgumentException($"{expression.ToString()} is not propery of {typeof(TMaterial).GetType().Name}.");
            }

            property = property.DeclaringType.GetProperty(property.Name);

            if (property == null)
            {
                return GetProperty(type.BaseType, expression);
            }

            return property;
        }
    }   

    internal static class Extentions
    {
        private static Dictionary<PropertyInfo, PropertyAccessor> _propertyAccessorMap = new Dictionary<PropertyInfo, PropertyAccessor>();

        private class PropertyAccessor
        {
            public PropertyAccessor(PropertyInfo property)
            {
                var method = typeof(Extentions)
                    .GetMethod("CreateGetter", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(property.ReflectedType, property.PropertyType);

                var getter = (Func<object, object>)method.Invoke(null, new object[] { property });

                Get = getter;

                method = typeof(Extentions)
                    .GetMethod("CreateSetter", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(property.ReflectedType, property.PropertyType);

                var setter = (Action<object, object>)method.Invoke(null, new object[] { property });

                Set = setter;
            }

            public Func<object, object> Get { get; }
            public Action<object, object> Set { get; }
        }

        private static Func<object, object> CreateGetter<TObj, TValue>(PropertyInfo property)
        {
            if (!property.CanRead)
            {
                return obj => property.Get(obj);
            }

            var getDelegate =
                (Func<TObj, TValue>)Delegate.CreateDelegate(
                         typeof(Func<TObj, TValue>),
                         property.GetGetMethod(nonPublic: true));

            return obj =>
            {
                var value = getDelegate((TObj)obj);

                return value;
            };
        }

        private static Action<object, object> CreateSetter<TObj, TValue>(PropertyInfo property)
        {
            if (!property.CanWrite)
            {             
                return (obj, value) => { };
            }

            var setDelegate =
                (Action<TObj, TValue>)Delegate.CreateDelegate(
                         typeof(Action<TObj, TValue>),
                         property.GetSetMethod(nonPublic: true));

            return (obj, value) =>
            {
                if (value is TValue)
                {
                    setDelegate((TObj)obj, (TValue)value);
                }
                else if (CanChangeType<TValue>(value))
                {
                    if (Nullable.GetUnderlyingType(typeof(TValue)) != null)
                    {
                        value = Convert.ChangeType(value, typeof(TValue).GetGenericArguments()[0]);
                    }
                    else
                    {
                        value = Convert.ChangeType(value, typeof(TValue));
                    }

                    setDelegate((TObj)obj, (TValue)value);
                }
            };
        }

        private static bool CanChangeType<TValue>(object value)
        {
            var conversionType = typeof(TValue);

            if (conversionType == null
                || value == null
                || value == DBNull.Value
                || !(value is IConvertible))
            {
                return false;
            }

            return true;
        }

        public static object Get(this PropertyInfo property, object obj)
        {
            if (!_propertyAccessorMap.TryGetValue(property, out PropertyAccessor accessor))
            {
                accessor = new PropertyAccessor(property);
                _propertyAccessorMap[property] = accessor;
            }

            return accessor.Get(obj);
        }        

        public static void Set(this PropertyInfo property, object obj, object value)
        {
            if (!_propertyAccessorMap.TryGetValue(property, out PropertyAccessor accessor))
            {
                accessor = new PropertyAccessor(property);
                _propertyAccessorMap[property] = accessor;
            }

            accessor.Set(obj, value);
        }
    }
}
