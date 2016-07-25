using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Extension
{
    public interface ITransaction : IDisposable
    {
        object Session();
        void Complete();
    }
}
