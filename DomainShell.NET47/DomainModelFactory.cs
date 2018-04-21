using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace DomainShell
{   
    public interface IDomainModelFactory
    {
        T Create<T>() where T : class;
    }
}
