using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;

namespace DomainShell.Tests.Infrastructure.Cart
{
    public class CartItemReadObject
    {
        public string CartId { get; set; }
        public string CartItemId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
    }

    public class CartReader
    {
        public CartReader(Session session)
        {
            _session = session;
        }

        private Session _session;

        private DbCommand CreateDbCommand()
        {
            return _session.GetPort<DbConnection>().CreateCommand();
        }

        public CartItemReadObject[] GetCartItems(string customerId)
        {
            DbCommand command = CreateDbCommand();

            command.CommandText = @"
                select * from Cart
                left join CartItem on Cart.CartId = CartItem.CartId
                left join Product on CartItem.ProductId = Product.ProductId
                where Cart.CustomerId = @CustomerId and CartItem.CartId is not null
                order by CartItem.CartItemId
            ";

            DbParameter param = command.CreateParameter();
            param.ParameterName = "@CustomerId";
            param.Value = customerId;
            command.Parameters.Add(param);

            List<CartItemReadObject> items = new List<CartItemReadObject>();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    CartItemReadObject item = new CartItemReadObject();

                    item.CartId = reader["CartId"].ToString();
                    item.CartItemId = reader["CartItemId"].ToString();
                    item.ProductId = reader["ProductId"].ToString();
                    item.ProductName = reader["ProductName"].ToString();
                    item.Price = int.Parse(reader["Price"].ToString());
                    item.Number = int.Parse(reader["Number"].ToString());

                    items.Add(item);
                }

                return items.ToArray();
            }
        }

        public decimal GetPostage()
        {
            return 150;
        }

        public decimal GetTaxRate()
        {
            return 0.05m;
        }
    }
}
