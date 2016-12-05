using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface ICartItemReadDto
    {
        int CustomerId { get; set; }
        int CartNo { get; set; }
        int ProductId { get; set; }
        string ProductName { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }
        decimal TotalPrice { get; set; }
    }

    public interface ICartReadService
    {
        IEnumerable<ICartItemReadDto> GetCartItemList(int customerId);
    }
}
