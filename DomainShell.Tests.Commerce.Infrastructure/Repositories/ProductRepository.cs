using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public ProductRepository(ISession session)            
        {
            _session = session;
        }

        private ISession _session;

        public ProductEntity Find(int id)
        {
            throw new NotImplementedException();
        }
    }
}
