﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
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
            CartModel cart = new CartModel();
            cart.CartId = cartId;
            cart.CartItems = new List<CartItemModel>();
            cart.CartItems.Add(new CartItemModel { CartId = cartId, Product = new ProductModel { Price = 100 }, Number = 1 });

            return cart;
        }

        public void Save(CartModel cart)
        {
            cart.Accepted();
        }
    }
}
