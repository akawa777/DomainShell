using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.ObjectModel;

namespace FreestyleOrm.Core
{
    internal static class CoreExtensions
    {
        public static object Get(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj);
        }

        public static void Set(this PropertyInfo property, object obj, object value)
        {
            object convertedValue;

            try
            {
                convertedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(obj, convertedValue);
            }
            catch
            {
                try
                {
                    property.SetValue(obj, value);
                }
                catch
                {

                }
            }            
        }

        public static bool IsList(this Type type)
        {
            return IsList(type, out Type elementType);
        }

        public static bool IsList(this Type type, out Type elementType)
        {
            elementType = null;

            if (type == typeof(string)) return false;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType) continue;

                var defineType = interfaceType.GetGenericTypeDefinition();

                if (defineType != null && (defineType == typeof(List<>) || defineType == typeof(Collection<>)))
                {
                    elementType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        public static Dictionary<string, PropertyInfo> GetPropertyMap(this Type type, BindingFlags bindingFlags, PropertyTypeFilters propertyTypeFilters)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | bindingFlags);

            Dictionary<string, PropertyInfo> map = new Dictionary<string, PropertyInfo>();

            foreach (var property in properties)
            {
                if (propertyTypeFilters == PropertyTypeFilters.IgonreClass)
                {
                    if (property.PropertyType != typeof(string) && property.PropertyType.IsClass) continue;                    
                }
                else if (propertyTypeFilters == PropertyTypeFilters.OnlyClass)
                {
                    if (property.PropertyType == typeof(string) || property.PropertyType.IsPrimitive || property.PropertyType.IsValueType) continue;                    
                }

                map[property.Name] = property;
            }

            return map;
        }

        public static object Create(this Type type)
        {
            return Activator.CreateInstance(type, true);
        }

        public static string GetExpressionPath<TRoot, TTarget>(this Expression<Func<TRoot, TTarget>> expression) where TRoot : class
        {
            return GetExpressionPath(expression, out PropertyInfo property);
        }

        public static string GetExpressionPath<TRoot, TTarget>(this Expression<Func<TRoot, TTarget>> expression, out PropertyInfo property) where TRoot : class
        {
            property = null;

            string[] sections = expression.Body.ToString().Split('.');
            List<string> path = new List<string>();

            Dictionary<string, PropertyInfo> propertyMap = typeof(TRoot).GetPropertyMap(BindingFlags.Public, PropertyTypeFilters.All);

            foreach (var section in sections)
            {
                var targetSection = section;

                int arrayHolderIndex = targetSection.IndexOf("[");
                if (arrayHolderIndex != -1) targetSection = targetSection.Substring(0, arrayHolderIndex + 1);


                if (propertyMap.TryGetValue(targetSection, out property))
                {
                    path.Add(targetSection);

                    Type type;

                    if (property.PropertyType.IsList(out Type elementType))
                    {
                        type = elementType;
                    }
                    else
                    {
                        type = property.PropertyType;
                    }

                    propertyMap = type.GetPropertyMap(BindingFlags.Public, PropertyTypeFilters.All);
                }
            }

            return string.Join(".", path);
        }
    }

    internal enum PropertyTypeFilters
    {
        All,
        IgonreClass,
        OnlyClass
    }    
}