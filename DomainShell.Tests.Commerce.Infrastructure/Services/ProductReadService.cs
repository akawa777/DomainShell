using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class ProductReadService : IProductReadService
    {
        public ProductReadService(ISession session)
        {
            _session = session;
        }

        private ISession _session;

        public string GetName(int productId)
        {
            return "xxx";
        }

        public decimal GetPrice(int productId)
        {
            return 100m;
        }
    }
}
