using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Infrastructure;
using DomainShell.Tests.Commerce.Domain;
using DomainShell.Tests.Commerce.Domain.Contracts;

namespace DomainShell.Tests.Commerce.Infrastructure.Services
{
    public class CreditCardService : ICreditCardService
    {
        public void Pay(CreditCardValue creditCard, decimal price, string content)
        {
            
        }
    }
}
