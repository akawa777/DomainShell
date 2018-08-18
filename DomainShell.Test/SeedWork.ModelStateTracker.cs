using System;
using System.Linq;
using System.Collections.Generic;
using DomainShell;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Specialized;
using DomainShell.Kernels;

namespace DomainShell.Test
{
    public class ModelStateTrackerKernel : ModelStateTrackerKernelBase
    {
        protected override object CreateTag(object domainModel)
        {
            return null;
        }
    }
}