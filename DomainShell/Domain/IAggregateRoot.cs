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
        
    }

    public interface IDomainModel<TEntity>
    {
        void Map(TEntity entity);
    }
}
