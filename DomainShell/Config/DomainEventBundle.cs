using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Config
{
    public interface IDomainEventBundle
    {
        void Bundle(IDomainEventRegister register);
    }
}
