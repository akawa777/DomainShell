using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharpMvt.Production
{
    public static class Extensions
    {
        public static string GetCamelCaseFullName(this Type type)
        {
            string nameSpace = type.Namespace.ToFirstLower();
            string fullName = nameSpace == string.Empty ? type.Name.ToFirstLower() : $"{nameSpace}.{type.Name.ToFirstLower() }";

            return fullName;
        }

        public static string GetFullName(this Type type)
        {
            return string.IsNullOrEmpty(type.Namespace) ? type.Name : $"{type.Namespace}.{type.Name}";
        }

        public static string ToFirstLower(this string text)
        {
            if (text == null) throw new ArgumentException($"{nameof(text)} is null.");

            string firstWord = text.Length == 0 ? string.Empty : text[0].ToString().ToLower();
            string restWord = text.Length < 2 ? string.Empty : text.Substring(1, text.Length - 1);

            return $"{firstWord}{restWord}";
        }

        public static ITsTypeTranspiler Get(this Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap, Type type)
        {
            if (!tsTypeTranspilerMap.ContainsKey(type))
            {
                throw new InvalidOperationException($"{type.GetFullName()} TypeTranspiler is not found.");
            }

            return tsTypeTranspilerMap[type];
        }

        public static bool IsArray(this Type type, out Type elementType)
        {
            elementType = null;

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType) continue;
                if (interfaceType.GetGenericTypeDefinition() != typeof(IEnumerable<>)) continue;

                if (elementType == null) elementType = interfaceType.GetGenericArguments()[0];
                else throw new InvalidOperationException($"{type.GetFullName()} has multi elementTtype.");
            }

            return elementType != null;
        }
    }
}