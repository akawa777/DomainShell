using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DomainShell.Infrastructure;
using DomainShell.Tests.Domain.Models;

namespace DomainShell.Tests.Infrastructure.Repositries
{
    public class ProductRepository : IRepositroy<ProductModel>
    {
        public ProductRepository(Session session)
        {
            _session = session;
        }

        private Session _session;

        public ProductModel Get(string productId)
        {
            return new ProductModel();
        }

        public void Save(ProductModel product)
        {
            product.Accepted();
        }
    }
}
