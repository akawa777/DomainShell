using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface IProductReadModel
    {
        int ProductId { get; }
        string ProductName { get; }
        decimal Price { get; }
    }

    public interface IProductReadService : IInfrastructureService
    {
        IProductReadModel Find(int productId);
    }
}
