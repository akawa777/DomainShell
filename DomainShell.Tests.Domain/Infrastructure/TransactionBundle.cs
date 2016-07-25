using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Extension;

namespace DomainShell.Tests.Domain.Infrastructure
{
    internal class TransactionBundle : ITransactionBundle
    {
        public void Bundle(ITransactionRegister register)
        {            
            register.Set<IAggregateRoot>(() => new Transaction());
        }
    }
}
