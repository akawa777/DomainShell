using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Domain.Customer;
using DomainShell.Tests.Domain.Product;

namespace DomainShell.Tests.Infrastructure.Cart
{   
    public class CartRepository : IWriteRepository<CartModel>
    {
        public CartRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public CartModel Find(string cartId)
        {   
            return Get("Cart.CartId = @CartId", new { CartId = cartId });
        }

        public CartModel Get(string customerId)
        {
            return Get("Cart.CustomerId = @CustomerId", new { CustomerId = customerId });
        }

        private CartModel Get(string where, object parameters)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            string template = @"
                select * from Cart
                left join CartItem on Cart.CartId = CartItem.CartId
                left join Product on CartItem.ProductId = Product.ProductId
                where {{where}}
                order by CartItem.CartItemId
            ";

            string sql = new TextBuilder(template, new { where = where }).Generate();

            CartProxy proxy = db.Query<CartProxy>(sql, parameters)
                .Unique("CartId")
                .Each((current, row) =>
                {
                    row.Map(current, x => x.Customer).Do();

                    row.Map(current, x => x.CartItemList, "CartItemId")
                        .Unique("CartId", "CartItemId")
                        .Each(item =>row.Map(item, x => item.Product).Do())
                        .Do();
                    
                }).Single();

            return proxy == null ? null : new CartModel(proxy);
        }

        public void Save(CartModel cart)
        {
            if (cart.State == State.Added)
            {
                Create(cart);
                cart.Stored();
            }
            else if (cart.State == State.Stored)
            {
                Update(cart);
            }
        }

        private void Create(CartModel cart)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            db.Command<CartModel>("Cart", "CartId").Insert(cart);            

            foreach (CartItemModel item in cart.CartItems)
            {                
                db.Command<CartItemModel>("CartItem", "CartId", "CartItemId").Insert(item);
            }
        }

        private void Update(CartModel cart)
        {
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            db.Command<CartModel>("Cart", "CartId").Update(cart);

            db.ExecuteReader("delete from CartItem where CartId = @CartId", new Parameter("CartId", cart.CartId));

            foreach (CartItemModel item in cart.CartItems)
            {   
                db.Command<CartItemModel>("CartItem", "CartId", "CartItemId").Insert(item);
            }
        }
    }
}
