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
    public class CartRepository : IRepository<CartModel>
    {
        public CartRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public CartModel Find(string cartId)
        {   
            DbCommand command = _session.CreateCommand();
            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CartId";
            param.Value = cartId;

            return Get("Cart.CartId = @CartId", param);
        }

        public CartModel Get(string customerId)
        {
            DbCommand command = _session.CreateCommand();
            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;

            CartModel cart = Get("Cart.CustomerId = @CustomerId", param);            

            return cart;
        }

        private CartModel Get(string where, params DbParameter[] parameters)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = string.Format(@"
                select * from Cart
                left join CartItem on Cart.CartId = CartItem.CartId
                left join Product on CartItem.ProductId = Product.ProductId
                where {0}
                order by CartItem.CartItemId
            ", where);

            command.Parameters.AddRange(parameters);            

            using (DbDataReader reader = command.ExecuteReader())
            {
                CartRecord record = null;            

                while (reader.Read())
                {
                    if (record == null)
                    {
                        record = new CartRecord();

                        record.CartId = reader["CartId"].ToString();
                        record.CustomerId = reader["CustomerId"].ToString();
                        record.CartItemList = new List<CartItemRecord>();
                    }

                    CartItemRecord item = new CartItemRecord();

                    item.CartId = reader["CartId"].ToString();

                    if (reader["CartItemId"] == DBNull.Value)
                    {
                        continue;
                    }

                    item.CartItemId = reader["CartItemId"].ToString();
                    item.ProductId = reader["ProductId"].ToString();
                    item.Product = new ProductRecord
                    {
                        ProductId = reader["ProductId"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        Price = int.Parse(reader["Price"].ToString())
                    };
                    item.Number = int.Parse(reader["Number"].ToString());

                    record.CartItemList.Add(item);
                }

                return record == null ? null : new CartModel(record);
            }
        }

        public void Create(CartModel cart)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                insert into Cart (CustomerId) values (@CustomerId) 
            ";

            DbParameter param = command.CreateParameter();

            param.ParameterName = "@CustomerId";
            param.Value = cart.CustomerId;

            command.Parameters.Add(param);

            command.ExecuteNonQuery();

            command.CommandText = @"
                select CartId from Cart where ROWID = last_insert_rowid();         
            ";

            cart.CartId = command.ExecuteScalar().ToString();

            foreach (CartItemModel item in cart.CartItems)
            {
                command.CommandText = @"
                    insert into CartItem (CartId, CartItemId, ProductId, Number) values (@CartId, @CartItemId, @ProductId, @Number) 
                ";

                param = command.CreateParameter();
                param.ParameterName = "@CartId";
                param.Value = cart.CartId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CartItemId";
                param.Value = item.CartItemId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ProductId";
                param.Value = item.ProductId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Number";
                param.Value = item.Number;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
            }
        }

        public void Update(CartModel cart)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                delete from CartItem where CartId = @CartId;                    
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CartId";
            param.Value = cart.CartId;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();

            foreach (CartItemModel item in cart.CartItems)
            {
                command.CommandText = @"                    
                    insert into CartItem (CartId, CartItemId, ProductId, Number) values (@CartId, @CartItemId, @ProductId, @Number) 
                ";
                
                param = command.CreateParameter();
                param.ParameterName = "@CartId";
                param.Value = cart.CartId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@CartItemId";
                param.Value = item.CartItemId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@ProductId";
                param.Value = item.ProductId;
                command.Parameters.Add(param);

                param = command.CreateParameter();
                param.ParameterName = "@Number";
                param.Value = item.Number;
                command.Parameters.Add(param);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(CartModel cart)
        {
            DbCommand command = _session.CreateCommand();

            command.CommandText = @"
                delete from Cart where CartId = @CartId;   
                delete from CartItem where CartId = @CartId;                                    
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CartId";
            param.Value = cart.CartId;
            command.Parameters.Add(param);

            command.ExecuteNonQuery();
        }
    }
}
