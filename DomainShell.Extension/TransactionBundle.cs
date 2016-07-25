using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Extension
{
    public interface ITransactionBundle
    {
        void Bundle(ITransactionRegister register);
    }
}
