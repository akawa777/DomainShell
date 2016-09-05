using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dagent;
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
            DagentDatabase db = new DagentDatabase(_session.GetPort<DbConnection>());

            List<CartItemReadObject> list = db.Query<CartItemReadObject>(@"
                select * from Cart
                left join CartItem on Cart.CartId = CartItem.CartId
                left join Product on CartItem.ProductId = Product.ProductId
                where Cart.CustomerId = @CustomerId and CartItem.CartId is not null
                order by CartItem.CartItemId
            ", new { CustomerId = customerId }).List();

            return list.ToArray();
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
