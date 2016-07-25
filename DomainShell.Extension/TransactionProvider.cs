using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Extension.Config;

namespace DomainShell.Extension
{
    public static class TransactionProvider
    {
        private static TransactionContainer _container = new TransactionContainer();
        private static Dictionary<string, bool> _assemblyMap = new Dictionary<string, bool>();

        private static void Bundle(Type aggregateRootType)
        {
            Assembly assembly = aggregateRootType.Assembly;

            if (_assemblyMap.ContainsKey(assembly.FullName))
            {
                return;
            }

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterface(typeof(ITransactionBundle).FullName) != null)
                {
                    ITransactionBundle bundle = Activator.CreateInstance(type) as ITransactionBundle;
                    bundle.Bundle(_container);
                }
            }

            _assemblyMap[assembly.FullName] = true;
        }

        public static ITransaction BeginTran<TAggregateRoot>()
        {
            return BeginTran(typeof(TAggregateRoot));
        }

        public static ITransaction BeginTran(Type aggregateRootType)
        {
            Bundle(aggregateRootType);
            return _container.BeginTran(aggregateRootType);
        }
    }
}
