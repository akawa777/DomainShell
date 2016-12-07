using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Domain.Services
{
    public class ProductReadService : IProductReadService
    {
        public ProductReadService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private IProductRepository _productRepository;

        public ProductEntity Find(int productId)
        {
            return _productRepository.Find(productId);
        }
    }
}
