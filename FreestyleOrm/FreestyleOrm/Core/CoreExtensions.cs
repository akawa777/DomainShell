using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            }
            catch
            {
                return;
            }

            property.SetValue(obj, value);
        }

        public static bool IsList(this Type type)
        {   
            if (type == typeof(string)) return false;

            if (type.IsArray) return true;

            foreach (var interfaceType in type.GetInterfaces())
            {
                var defineType = interfaceType.GetGenericTypeDefinition();

                if (defineType != null && (defineType == typeof(IList<>) || defineType == typeof(ICollection<>)))
                {                    
                    return true;
                }
            }

            if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) return true;

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

        public static string GetEntityPath<TRoot, TTarget>(this Expression<Func<TRoot, TTarget>> expression, out PropertyInfo property) where TRoot : class
        {
            property = null;

            string[] sections = expression.ToString().Split('.');
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
                    propertyMap = property.PropertyType.GetPropertyMap(BindingFlags.Public, PropertyTypeFilters.All);
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