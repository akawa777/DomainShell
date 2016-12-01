using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Infrastructure.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class CartReader : ICartReader
    {
        public CartReader(ISession session)
        {
            _session = session;
        }

        private ISession _session;

        public IEnumerable<CartItemReadDto> GetCartItemList(int customerId)
        {
            return Enumerable.Empty<CartItemReadDto>();
        }
    }
}
