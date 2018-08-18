using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IOpenScope : IDisposable
    {
        
    }

    public interface ITranScope : IDisposable
    {
        void Complete();
    }

    public interface ISessionKernel
    {
        IOpenScope Open();
        ITranScope Tran();
    }
}