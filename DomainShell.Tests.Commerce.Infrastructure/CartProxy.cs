using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Infrastructure.Shared;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class CartProxy : CartEntity, IAggregateProxyModel, IVersion
    {   
        public CartProxy(int customerId)
            : base(customerId)
        {

        }

        public bool Transient
        {
            get;
            set;
        }

        public new List<CartItemEntity> CartItemList
        {
            get
            {
                return _cartItemList;
            }
            set
            {
                _cartItemList = value;
            }
        }

        public bool OnceVerified
        {
            get;
            set;
        }

        public bool Deleted
        {
            get;
            set;
        }

        public override void Validate(DomainShell.Domain.IValidationSpec<CartEntity> spec)
        {
            base.Validate(spec);
            OnceVerified = true;
        }

        public int Version
        {
            get;
            set;
        }
    }
}
