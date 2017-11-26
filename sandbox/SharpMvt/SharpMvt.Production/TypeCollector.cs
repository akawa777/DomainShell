using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SharpMvt.Production
{
    public class TypeCollector
    {
        public Type[] GetTypes(string dllsDir)
        {
            Console.WriteLine($"{nameof(dllsDir)} : {dllsDir}");

            List<Assembly> assemblies = new List<Assembly>();
            List<Type> types = new List<Type>();

            foreach (var dll in Directory.GetFiles(dllsDir, "*.dll"))
            {
                assemblies.Add(Assembly.LoadFrom(dll));
            }

            foreach (var assembly in assemblies)
            {
                types.AddRange(assembly.GetTypes());
            }

            return types.ToArray();
        }
    }
}
