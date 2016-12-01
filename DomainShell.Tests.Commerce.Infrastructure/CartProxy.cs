using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class CartProxy : CartEntity, IProxyModel, ITransient
    {
        public CartProxy(CartId id) : base(id)
        {

        }

        public bool Transient
        {
            get;
            set;
        }
    }
}
