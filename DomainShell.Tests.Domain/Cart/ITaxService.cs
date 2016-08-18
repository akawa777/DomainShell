using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Domain;

namespace DomainShell.Tests.Domain.Cart
{
    public interface ITaxService
    {
        decimal GetTax(decimal amount);
        decimal Calculate(decimal amount);
    }
}
