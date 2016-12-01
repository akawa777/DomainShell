using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Commerce.Domain.Contracts
{
    public interface ICreditCardService : IInfrastructureService
    {
        void Pay(int cardCompanyId, int cardNo, decimal price, string content);
    }
}
