﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell
{
    public interface IDomainEventCacheKernel<TDomainEvent> : IList<TDomainEvent>
    {

    }
}
