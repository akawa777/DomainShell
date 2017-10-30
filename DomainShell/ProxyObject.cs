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
            object material = Activator.CreateInstance(typeof(TMaterial), true);

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
            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo propertyInfo)
                {
                    var material = (TProperty)propertyInfo.Get(Material);
                    return new ProxyObject<TProperty>(material, propertyInfo);
                }
            }

            throw new ArgumentException($"{expression.ToString()} is not propery of {typeof(TMaterial).GetType().Name}.");
        }

        public IEnumerable<IProxyObject<TProperty>> List<TProperty>(Expression<Func<TMaterial, IEnumerable<TProperty>>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo propertyInfo)
                {
                    var property = (expression.Body as MemberExpression).Member as PropertyInfo;
                    var materials = (IEnumerable<TProperty>)propertyInfo.Get(Material);         
                    return materials.Select(x => new ProxyObject<TProperty>(x, null));
                }
            }

            throw new ArgumentException($"{expression.ToString()} is not propery of {typeof(TMaterial).GetType().Name}.");    
        }

        public IProxyObject<TMaterial> Set<TProperty>(Expression<Func<TMaterial, TProperty>> expression, Func<TMaterial, PropertyInfo, object> value)
        {
            var property = (expression.Body as MemberExpression).Member as PropertyInfo;   
            property.Set(Material, value(Material, Property));
            return this;
        }
    }   

    internal static class Extentions
    {
        private static Dictionary<PropertyInfo, PropertyAccessor> _propertyAccessorMap = new Dictionary<PropertyInfo, PropertyAccessor>();

        private class PropertyAccessor
        {
            public PropertyAccessor(PropertyInfo property)
            {
                MethodInfo method = typeof(Extentions)
                    .GetMethod("CreateGetter", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(property.ReflectedType, property.PropertyType);

                Func<object, object> getter = (Func<object, object>)method.Invoke(null, new object[] { property });

                Get = getter;

                method = typeof(Extentions)
                    .GetMethod("CreateSetter", BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(property.ReflectedType, property.PropertyType);

                Action<object, object> setter = (Action<object, object>)method.Invoke(null, new object[] { property });

                Set = setter; ;
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

            Func<TObj, TValue> getDelegate =
                (Func<TObj, TValue>)Delegate.CreateDelegate(
                         typeof(Func<TObj, TValue>),
                         property.GetGetMethod(nonPublic: true));

            return obj =>
            {
                TValue value = getDelegate((TObj)obj);

                return value;
            };
        }

        private static Action<object, object> CreateSetter<TObj, TValue>(PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                return (obj, value) => property.Set(obj, value);
            }

            Action<TObj, TValue> setDelegate =
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
            Type conversionType = typeof(TValue);

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
            PropertyAccessor accessor;

            if (!_propertyAccessorMap.TryGetValue(property, out accessor))
            {
                accessor = new PropertyAccessor(property);
                _propertyAccessorMap[property] = accessor;
            }

            return accessor.Get(obj);
        }        

        public static void Set(this PropertyInfo property, object obj, object value)
        {
            PropertyAccessor accessor;

            if (!_propertyAccessorMap.TryGetValue(property, out accessor))
            {
                accessor = new PropertyAccessor(property);
                _propertyAccessorMap[property] = accessor;
            }

            accessor.Set(obj, value);
        }
    }
}
