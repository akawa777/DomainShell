using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure
{
    public class CartProxy : CartEntity, IAggregateProxyModel, ITransient
    {
        public CartProxy(CartId id) : base(id)
        {

        }

        public bool Transient { get; set; }

        public bool Deleted { get; private set; }

        public bool OnceVerified { get; set; }

        public override void Purchase(CreditCardValue creditCard, ICreditCardService creditCardService, IProductReadService productReadService, IValidationSpec<CartEntity> spec)
        {
            base.Purchase(creditCard, creditCardService, productReadService, spec);
            
            OnceVerified = true;
        }

        public override void Validate(IValidationSpec<CartEntity> spec)
        {
            base.Validate(spec);

            OnceVerified = true;
        }

        public override void Delete()
        {
            base.Delete();

            Deleted = true;
        }
    }
}
