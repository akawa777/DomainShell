using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{
    public interface IDomainModelProxy
    {
        Type GetImplementType();
    }
    
    public interface IDomainModelProxyFactory
    {
        T Create<T>() where T : class;
    }
}
