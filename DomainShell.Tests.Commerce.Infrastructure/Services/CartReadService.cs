using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Infrastructure.Daos;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class CartReadService : ICartReadService
    {
        public CartReadService(ISession session)
        {
            _cartDao = new CartDao(session);
        }

        private CartDao _cartDao;

        public IEnumerable<ICartItemReadDto> GetCartItemList(int customerId)
        {
            return _cartDao.GetCartItemList(customerId);
        }
    }
}
