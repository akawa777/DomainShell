﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DomainShell.Domain
{   
    public interface IAggregateRoot
    {
        State State { get; }
        void Stored();
    }

    public enum State
    {
        Added,
        Stored,
        Removed
    }
}
