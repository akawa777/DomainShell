using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DomainShell.Extension.Config;
using DomainShell.Tests.Domain.Infrastructure;

namespace DomainShell.Tests.Domain.Config
{
    public class TransactionBundle : ITransactionBundle
    {
        public void Bundle(ITransactionRegister register)
        {            
            register.Set<IAggregateRoot>(() => new Transaction());
        }
    }
}
