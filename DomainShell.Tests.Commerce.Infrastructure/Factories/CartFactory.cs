using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Factories
{
    public class CartFactory : ICartFactory
    {
        public CartFactory(ISession session)
        {
            _session = session;
        }

        private ISession _session;

        public CartEntity Create(ICreationSpec<CartEntity, CartConstructorParameters> spec)
        {
            CartConstructorParameters parameters = spec.ConstructorParameters();

            CartProxy cart = new CartProxy(new CartId(parameters.CustomerId));
            cart.Transient = true;

            spec.Satisfied(cart);

            return cart;
        }
    }
}
