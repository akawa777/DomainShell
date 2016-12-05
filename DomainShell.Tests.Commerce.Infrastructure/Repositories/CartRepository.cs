using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;
using DomainShell.Tests.Commerce.Infrastructure.Shared;
using DomainShell.Tests.Commerce.Infrastructure.Daos;

namespace DomainShell.Tests.Commerce.Infrastructure.Repositories
{
    public class CartRepository : BaseRepository<CartEntity, CartId>, ICartRepository
    {
        public CartRepository(ISession session, IDomainEventDispatcher domainEventDispatcher)
            : base(domainEventDispatcher)
        {
            _session = session;
            _cartDao = new CartDao(session);
        }

        private ISession _session;        
        private CartDao _cartDao;

        public override CartEntity Find(CartId id)
        {
            return _cartDao.Find(id);
        }

        public override void Insert(CartEntity aggregateRoot)
        {
            _cartDao.Insert(aggregateRoot);
        }

        public override void Update(CartEntity aggregateRoot)
        {
            _cartDao.Update(aggregateRoot);
        }

        public override void Delete(CartEntity aggregateRoot)
        {
            _cartDao.Delete(aggregateRoot);
        }
    }
}
