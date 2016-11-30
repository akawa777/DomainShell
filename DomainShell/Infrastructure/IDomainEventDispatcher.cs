﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Infrastructure
{  
    public interface IDomainEventDispatcher
    {
        void Dispatch(IDomainEvent domainEvent);
    }
}