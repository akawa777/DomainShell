using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Tests.Domain.Cart;

namespace DomainShell.Tests.Infrastructure.Cart
{
    public class CartReader
    {
        public CartReader(Session session)
        {
            _session = session;
        }

        private Session _session;

        public List<CartReadModel> GetAll()
        {
            return new List<CartReadModel>();
        }

        public List<CartItemReadModel> GetDetailList(string cartId)
        {
            return new List<CartItemReadModel>();
        }
    }
}
