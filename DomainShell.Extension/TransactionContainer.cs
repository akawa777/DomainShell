using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Extension
{
    public interface ITransactionRegister
    {
        void Set(Type aggregateRootType, Func<ITransaction> beginTran);
        void Set<TAggregateRoot>(Func<ITransaction> beginTran) where TAggregateRoot : IAggregateRoot;
    }

    internal interface ITransactionLoder
    {
        ITransaction BeginTran<TAggregateRoot>();
        ITransaction BeginTran(Type aggregateRootType);
    }

    internal class TransactionContainer : ITransactionRegister, ITransactionLoder
    {
        private Dictionary<Type, Func<ITransaction>> _tranMap = new Dictionary<Type, Func<ITransaction>>();

        public void Set(Type aggregateRootType, Func<ITransaction> beginTran)
        {
            _tranMap[aggregateRootType] = beginTran;
        }

        public void Set<TAggregateRoot>(Func<ITransaction> beginTran) where TAggregateRoot : IAggregateRoot
        {
            _tranMap[typeof(TAggregateRoot)] = beginTran;
        }

        public ITransaction BeginTran<TAggregateRoot>()
        {
            return BeginTran(typeof(TAggregateRoot));
        }

        public ITransaction BeginTran(Type aggregateRootType)
        {
            if (_tranMap.ContainsKey(aggregateRootType))
            {
                return _tranMap[aggregateRootType]();
            }

            foreach (Type type in aggregateRootType.GetInterfaces())
            {
                return BeginTran(type);
            }

            return null;
        }
    }
}
