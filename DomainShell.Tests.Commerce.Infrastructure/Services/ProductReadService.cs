using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class ProductReadModel : IProductReadModel
    {
        public int ProductId
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }
    }

    public class ProductReadService : IProductReadService
    {
        public ProductReadService(ISession session)
        {
            _session = session;
        }

        private ISession _session;

        public IProductReadModel Find(int productId)
        {
            return new ProductReadModel
            {
                ProductId = productId,
                ProductName = "xxx",
                Price = 100m
            };
        }
    }
}
