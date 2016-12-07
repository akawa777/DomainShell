using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface IProductReadService
    {
        ProductEntity Find(int productId);
    }
}
