using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Commerce.Infrastructure.Contracts
{
    public interface ICartReadService
    {
        IEnumerable<CartItemReadDto> GetCartItemList(int customerId);
    }
}
