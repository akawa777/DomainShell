using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;

namespace DomainShell.Tests.Infrastructure.Cart
{
    public class CartRepository : IRepositroy<CartModel>
    {
        public CartRepository(Session session)
        {
            _session = session;
        }

        private Session _session;  

        public CartModel Get(string cartId)
        {
            return new CartModel();
        }

        public void Save(CartModel cart)
        {
            if (cart.State == State.Created)
            {
                Create(cart);
            } 
            else if (cart.State == State.Updated)
            {
                Update(cart);
            }
            else if (cart.State == State.Deleted)
            {
                Delete(cart);
            }

            cart.Accepted();
        }

        private void Create(CartModel cart)
        {
            
        }

        private void Update(CartModel cart)
        {

        }

        private void Delete(CartModel cart)
        {

        }

    }
}
