using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Domain
{
    public enum State
    {
        UnChanged = 0,        
        Modified = 1,
        Deleted = 2
    }
}
