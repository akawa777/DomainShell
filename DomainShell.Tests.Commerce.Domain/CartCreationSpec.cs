using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain
{
    public class CartConstructorParameters
    {
        public int CustomerId { get; set; }
    }

    public class CartCreationSpec : ICreationSpec<CartEntity, CartConstructorParameters>
    {
        public CartCreationSpec(int customerId)
        {
            _customerId = customerId;
        }

        private int _customerId;

        public CartConstructorParameters ConstructorParameters()
        {
            return new CartConstructorParameters
            {
                CustomerId = _customerId
            };
        }

        public void Satisfied(CartEntity target)
        {
            
        }
    }
}
