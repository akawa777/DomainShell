using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;
using DomainShell.Tests.Domain.Payment;

namespace DomainShell.Tests.Infrastructure.Payment
{
    public class CreditCardService : ICreditCardService
    {
        public void Pay(CreditCardValue creditCard, decimal paymentAmount)
        {
            
        }
    } 
}
