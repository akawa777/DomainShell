using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface IProductReadService : IInfrastructureService
    {
        string GetName(int productId);
        decimal GetPrice(int productId);
        bool Exist(int productId);
    }
}
