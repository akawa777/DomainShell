using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainShell.Tests.Domain.Cart;
using DomainShell.Tests.Infrastructure;

namespace DomainShell.Tests.Infrastructure.Cart
{
    public class TaxService : ITaxService
    {
        public TaxService(Session session)
        {
            _cartReader = new CartReader(session);
        }

        public CartReader _cartReader;

        public decimal GetTax(decimal amount)
        {
            return amount * _cartReader.GetTaxRate();
        }

        public decimal Calculate(decimal amount)
        {
            return amount + GetTax(amount);
        }
    }
}
