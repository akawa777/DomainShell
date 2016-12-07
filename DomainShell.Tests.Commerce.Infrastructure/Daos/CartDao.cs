using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dagent;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;

namespace DomainShell.Tests.Commerce.Infrastructure.Daos
{
    public class CartDao
    {
        public CartDao(ISession session)
        {
            _connection = session.GetPort<System.Data.Common.DbConnection>();
        }

        private System.Data.Common.DbConnection _connection;

        private string _templateSql = @"
            select
                *
            from
                Cart
            left join
                CartItem
            on
                Cart.CustomerId = CartItem.CustomerId
            {{where}}                
            order by
                Cart.CustomerId,
                CartItem.CartItemNo
        ";

        private string _indent = "                ";

        public CartProxy Find(CartId id)
        {
            List<Parameter> parameterList = new List<Parameter>();

            string where = "where" + Environment.NewLine ;
            where += _indent + "Cart.CustomerId = @customerId";

            parameterList.Add(new Parameter("customerId", id.CustomerId));

            TextBuilder tb = new TextBuilder(_templateSql, new { where = where });

            string sql = tb.Generate();

            DagentDatabase db = new DagentDatabase(_connection);

            CartProxy cart = db.Query<CartProxy>(sql, parameterList.ToArray())
                .Unique("CustomerId")
                .Create(row => new CartProxy(row.Get<int>("CustomerId")))
                .Each((model, row) =>
                {  
                    row.Map(model, x => x.CartItemList, "CartItemNo")
                        .Create(() => new CartItemEntity(row.Get<int>("CustomerId"), row.Get<int>("CartItemNo")))
                        .Do();
                })
                .Single();

            return cart;
        }

        private ICommand<CartEntity> CreateCartCommand(DagentDatabase database)
        {
            return 
                database.Command<CartEntity>("Cart", "CustomerId")
                .Map((row, entity) =>
                {
                    row["CustomerId"] = entity.Id.CustomerId;
                });
        }

        private ICommand<CartItemEntity> CreateCartItemCommand(DagentDatabase database)
        {
            return 
                database.Command<CartItemEntity>("CartItem", "CustomerId", "CartItemNo")
                .Map((row, entity) =>
                {
                    row["CustomerId"] = entity.Id.CartId.CustomerId;
                    row["CartItemNo"] = entity.Id.CartItemNo;
                });
        }

        public void Insert(CartEntity cart)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            ICommand<CartEntity> cartCommand = CreateCartCommand(db);

            cartCommand.Insert(cart);

            ICommand<CartItemEntity> cartItemCommand = CreateCartItemCommand(db);

            foreach (CartItemEntity cartItem in cart.CartItemList)
            {
                cartItemCommand.Insert(cartItem);
            }
        }

        public void Update(CartEntity cart)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            ICommand<CartEntity> cartCommand = CreateCartCommand(db);

            cartCommand.Update(cart);

            ICommand<CartItemEntity> cartItemCommand = CreateCartItemCommand(db);

            foreach (CartItemEntity cartItem in cart.CartItemList)
            {
                cartItemCommand.Update(cartItem);
            }
        }

        public void Delete(CartEntity cart)
        {
            DagentDatabase db = new DagentDatabase(_connection);

            ICommand<CartEntity> cartCommand = CreateCartCommand(db);

            cartCommand.Delete(cart);

            db.ExequteNonQuery("delete from CartItem where CustomerId = @customerId", new Parameter("customerId", cart.Id.CustomerId));
        }

        public IEnumerable<CartItemReadDto> GetCartItemList(int customerId)
        {
            List<Parameter> parameterList = new List<Parameter>();

            string where = "where" + Environment.NewLine;
            where += _indent + "Cart.CustomerId = @customerId";

            parameterList.Add(new Parameter("customerId", customerId));

            TextBuilder tb = new TextBuilder(_templateSql, new { where = where });

            string sql = tb.Generate();

            DagentDatabase db = new DagentDatabase(_connection);

            IEnumerable<CartItemReadDto> list = db.Query<CartItemReadDto>(sql, parameterList.ToArray()).EnumerateList();

            return list;
        }
    }
}
